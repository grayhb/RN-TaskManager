using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using RN_TaskManager.DAL.Context;
using RN_TaskManager.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace RN_TaskManager.DAL.Repositories
{
    public class MailRepository : BaseRepository<Mail>, IMailRepository
    {
        private readonly RN_TaskManagerContext _context;
        private readonly ILogger _logger;

        private SmtpClient _smtp;

        private string _systemName;
        private string _systemUrl;

        private string _emailHost;
        private string _emailPort;
        private string _emailLogin;
        private string _emailPassword;
        private string _emailAddress;

        public MailRepository(RN_TaskManagerContext context, IConfiguration configuration, ILogger<MailRepository> logger) : base(context)
        {
            _context = context;
            _logger = logger;

            _systemName = configuration["SystemName"];
            _systemUrl = configuration["SystemUrl"];

            _emailHost = configuration["Email:Host"];
            _emailPort = configuration["Email:Port"];
            _emailLogin = configuration["Email:Login"];
            _emailPassword = configuration["Email:Password"];
            _emailAddress = configuration["Email:Address"];
        }

        public async Task CreateMailsAsync()
        {
            var lastItem = await _context.Mails.OrderByDescending(e => e.DateCreate).FirstOrDefaultAsync();

            if (lastItem != null && lastItem.DateCreate.ToString("dd.MM.yyyy") == DateTime.Now.ToString("dd.MM.yyyy"))
                return;

            // формируем список заданий
            var allowStatuses = new int[] { 2, 3 };
            var dateNow = DateTime.Now;
            var dateTomorrow = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day).AddDays(1);

            var tasks = await _context.ProjectTasks
                .Include(e => e.ProjectTaskPerformers)
                .ThenInclude(e => e.User)
                .Include(e => e.Project)
                .Include(e => e.ProjectTaskType)
                .Where(e =>
                    !e.Deleted
                    && e.ProjectTaskStatusId != null && allowStatuses.Any(s => s == e.ProjectTaskStatusId.Value)
                    && e.EndFact == null && e.StartPlan != null && e.EndPlan != null
                    && e.EndPlan < dateTomorrow
                    )
                .ToListAsync();

            if (tasks.Count == 0)
                return;

            // формируем уникальный список пользователей
            var users = new List<User>();

            foreach (var task in tasks)
            {
                foreach (var performer in task.ProjectTaskPerformers.Where(e => !e.Deleted))
                {
                    if (!users.Any(e => e.UserId == performer.UserId))
                        users.Add(performer.User);
                }
            }

            var newMails = new List<Mail>();

            // формируем письмо для пользователя
            foreach (var user in users.Where(e => !string.IsNullOrEmpty(e.Email)))
            {
                var body = $"{user.FirstName} {user.Patronymic}!<br />";
                body += "Вам необходимо обратить внимание на следующие просроченные задачи:<br /><br />";
                body += "<table border=\"1\" width=\"100%\" >";
                body += "<thead>";
                body += "<tr><th>Проект</th><th>Описание задачи</th><th>Плановая дата</th><th></th></tr>";
                body += "</thead>";

                body += "<tbody>";

                foreach (var task in tasks
                    .Where(e => e.ProjectTaskPerformers.Any(p => p.UserId == user.UserId && !e.Deleted))
                    .OrderBy(e => e.EndPlan)
                    )
                {
                    body += "<tr>";
                    body += $"<td>{task.Project.ProjectName}</td>";
                    body += $"<td>{task.Details}</td>";
                    body += $"<td>{task.EndPlan.Value:dd.MM.yyyy}</td>";
                    body += $"<td><a href=\"{_systemUrl}/#/project-tasks/{task.ProjectTaskId}\" target=\"_blank\">открыть</a> </td>";
                    body += "</tr>";
                }
                body += "</tbody>";

                body += "</table>";
                body += "<br /><br /><i>Это письмо создано автоматически, отвечать на него не нужно.</i>";
                body += "<br />- - -<br />";
                body += $"<a href=\"{_systemUrl}\" target=\"_blank\"><b>{_systemName}</b></a>";

                body = $"<font size=\"3\" color=\"#000066\" face=\"Trebuchet MS, Tahoma\">{body}</font>";

                newMails.Add(new Mail()
                {
                    Topic = "Просроченные задания",
                    Body = body,
                    DateCreate = DateTime.Now,
                    Address = user.Email,
                });

            }

            if (newMails.Count > 0)
            {
                _context.Mails.AddRange(newMails);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SendMails() {
            var items = await _context.Mails.Where(e => e.DateSend == null).ToListAsync();

            if (items.Count == 0)
                return;

            InitSmtp();

            foreach(var item in items)
            {
                SendMail(item);
            }

            var needUpdateItems = items.Where(e => e.DateSend != null).ToList();

            if (needUpdateItems.Count > 0)
            {
                _context.Mails.UpdateRange(needUpdateItems);
                await _context.SaveChangesAsync();
            }

        }

        private void InitSmtp()
        {
            _smtp = new SmtpClient(_emailHost, int.Parse(_emailPort));
            _smtp.Credentials = new NetworkCredential(_emailLogin, _emailPassword);
            _smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        }

        private void SendMail(Mail mail)
        {
            MailMessage Message = new MailMessage();
            Message.From = new MailAddress(_emailAddress);
            Message.To.Add(new MailAddress(mail.Address));
            Message.Subject = mail.Topic;
            Message.Body = mail.Body;
            Message.IsBodyHtml = true;

            try
            {
                _smtp.Send(Message);
                mail.DateSend = DateTime.Now;
            }
            catch (SmtpFailedRecipientsException ex)
            {
                for (int i = 0; i < ex.InnerExceptions.Length; i++)
                {
                    SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                    if (status == SmtpStatusCode.MailboxBusy ||
                        status == SmtpStatusCode.MailboxUnavailable)
                    {
                        _logger.LogError("Ошибка отправки письма, повтор через 5 секунд");

                        System.Threading.Thread.Sleep(5000);
                        _smtp.Send(Message);
                    }
                    else
                    {
                        _logger.LogError($"Ошибка отправки сообщения - {ex.InnerExceptions[i].FailedRecipient}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка отправки сообщения - {ex.Message}");
            }
        }

    }
}
