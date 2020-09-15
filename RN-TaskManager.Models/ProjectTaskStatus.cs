using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RN_TaskManager.Models
{
    [Table("Statuses")]
    public class ProjectTaskStatus
    {
        [Key]
        public int ProjectTaskStatusId { get; set; }
        public string StatusName { get; set; }
    }
}
