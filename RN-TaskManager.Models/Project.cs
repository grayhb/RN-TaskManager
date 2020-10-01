using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RN_TaskManager.Models
{
    [Table("Projects")]
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }

        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }

        public int ProjectImportance { get; set; }

        public int? UserId { get; set; }
        public User Responsible { get; set; }

        public bool Deleted { get; set; }

        public string ResponsibleName => Responsible == null ? "" : Responsible.ShortName;


    }
}
