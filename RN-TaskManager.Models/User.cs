using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RN_TaskManager.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int UserId { get; set; }

        public Guid Guid { get; set; }

        public string Login { get; set; }
        
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Patronymic { get; set; }

        [ForeignKey("Group")]
        public int? GroupId { get; set; }
        public Group Group { get; set; }


        public bool Deleted { get; set; }

        /// <summary>
        /// Фамилия И.О.
        /// </summary>
        public string ShortName => $"{LastName} {FirstName?[0]}.{Patronymic?[0]}.";

        public string GroupName => Group == null ? "" : Group.GroupName;

    }
}
