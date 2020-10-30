using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RN_TaskManager.Models
{
    [Table("Mails")]
    public class Mail
    {
        [Key]
        public int MailId { get; set; }

        public DateTime DateCreate { get; set; }

        public string Address { get; set; }
        public string Body { get; set; }

        public string Topic { get; set; }

        public DateTime? DateSend { get; set; }

    }
}
