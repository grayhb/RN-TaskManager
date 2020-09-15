using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RN_TaskManager.Models
{
    [Table("ProjectTaskTypes")]
    public class ProjectTaskType
    {
        [Key]
        public int ProjectTaskTypeId { get; set; }

        public string ProjectTaskTypeName { get; set; }

        public int Order { get; set; }

        public bool Deleted { get; set; }

    }
}
