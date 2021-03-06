﻿using Microsoft.EntityFrameworkCore;
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

        private readonly string _systemName;
        private readonly string _systemUrl;

        private readonly string _emailHost;
        private readonly string _emailPort;
        private readonly string _emailLogin;
        private readonly string _emailPassword;
        private readonly string _emailAddress;

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

        /// <summary>
        /// Создание письма о новой задаче для пользователя
        /// </summary>
        public async Task CreateMailForNewPerformerAsync(User user, ProjectTask projectTask)
        {
            if (projectTask == null || user == null)
                return;

            await _context.Mails.AddAsync(GetMailForPerformer(user, projectTask));
            await _context.SaveChangesAsync();
        }

        public async Task CreateMailsForNewPerformersAsync(List<User> users, ProjectTask projectTask)
        {
            if (projectTask == null || users.Count == 0)
                return;

            var mails = new List<Mail>();

            foreach (var user in users)
                mails.Add(GetMailForPerformer(user, projectTask));

            await _context.Mails.AddRangeAsync(mails);
            await _context.SaveChangesAsync();
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

            // формируем письма для пользователей
            foreach (var user in users.Where(e => !string.IsNullOrEmpty(e.Email)))
                newMails.Add(GetMailForTask(user, tasks));

            if (newMails.Count > 0)
            {
                await _context.Mails.AddRangeAsync(newMails);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SendMails()
        {
            var items = await _context.Mails.Where(e => e.DateSend == null).ToListAsync();

            if (items.Count == 0)
                return;

            InitSmtp();

            foreach (var item in items)
                SendMail(item);

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
            MailMessage Message = new MailMessage
            {
                From = new MailAddress(_emailAddress),
                Subject = mail.Topic,
                Body = mail.Body,
                IsBodyHtml = true
            };

            Message.To.Add(new MailAddress(mail.Address));

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

        private Mail GetMailForPerformer(User user, ProjectTask projectTask)
        {
            var body = $"{user.FirstName} {user.Patronymic}!<br />" +
                       $"Для Вас назначена новая задача:<br /><br />" +
                       $"<p>Проект: <b>{projectTask.Project.ProjectName}</b></p>" +
                       $"<p>Описание задачи: <b>{projectTask.Details}</b></p>" +
                       $"<p>Плановая дата начала работ: " + (projectTask.StartPlan != null ? $"{projectTask.StartPlan.Value:dd.MM.yyyy}" : $"-") + "</p>" +
                       $"<p>Плановая дата окончания работ: " + (projectTask.EndPlan != null ? $"{projectTask.EndPlan.Value:dd.MM.yyyy}" : $"-") + "</p>" +
                       $"<p><a href=\"{_systemUrl}/#/project-tasks/{projectTask.ProjectTaskId}\" target=\"_blank\">[открыть карточку задачи]</a></p>" +

                       $"<br /><br /><i>Это письмо создано автоматически, отвечать на него не нужно.</i> <br />- - -<br />" +
                       $"<a href=\"{_systemUrl}\" target=\"_blank\"><b>{_systemName}</b></a>";

            body = $"<div style=\"font-sze:10px; color:#000066; font-family:Trebuchet MS, Tahoma\">{body}</div>";

            var newMail = new Mail()
            {
                Topic = $"Новое задание №{projectTask.ProjectTaskId}",
                Body = body,
                DateCreate = DateTime.Now,
                Address = user.Email,
            };

            return newMail;
        }

        private Mail GetMailForTask(User user, List<ProjectTask> tasks)
        {
            var body = $"{user.FirstName} {user.Patronymic}!<br />";
            body += "Вам необходимо обратить внимание на следующие просроченные задачи:<br /><br />";
            body += "<table border=\"1\" width=\"100%\" style=\"font-family:Trebuchet MS, Tahoma; border-collapse: collapse; border: 1px solid #000066;\" >";
            body += "<thead>";
            body += "<tr><th>Проект</th><th>Описание задачи</th><th>Плановая дата</th><th></th></tr>";
            body += "</thead>";

            body += "<tbody>";

            foreach (var task in tasks
                .Where(e => e.ProjectTaskPerformers.Any(p => p.UserId == user.UserId && !p.Deleted))
                .OrderBy(e => e.EndPlan)
                )
            {
                body += "<tr>";
                body += $"<td>{task.Project.ProjectName}</td>";
                body += $"<td>{task.Details}</td>";

                if (task.EndPlan != null)
                    body += $"<td>{task.EndPlan.Value:dd.MM.yyyy}</td>";
                else
                    body += $"<td></td>";

                body += $"<td><a href=\"{_systemUrl}/#/project-tasks/{task.ProjectTaskId}\" target=\"_blank\">открыть</a> </td>";
                body += "</tr>";
            }
            body += "</tbody>";

            body += "</table>";
            body += "<br /><br /><i>Это письмо создано автоматически, отвечать на него не нужно.</i>";
            body += "<br />- - -<br />";
            body += $"<a href=\"{_systemUrl}\" target=\"_blank\"><b>{_systemName}</b></a>";

            body = $"<font size=\"3\" color=\"#000066\" face=\"Trebuchet MS, Tahoma\">{body}</font>";

            var newMail = new Mail()
            {
                Topic = "Просроченные задания",
                Body = body,
                DateCreate = DateTime.Now,
                Address = user.Email,
            };

            return newMail;
        }
    }
}
