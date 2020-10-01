using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RN_TaskManager.Models
{
    [Table("TaskTypes")]
    public class TaskType
    {
        [Key]
        public int TaskTypeId { get; set; }

        public string TaskTypeName { get; set; }
        public string Note { get; set; }

        public int Order { get; set; }

        public bool Deleted { get; set; }
    }
}
