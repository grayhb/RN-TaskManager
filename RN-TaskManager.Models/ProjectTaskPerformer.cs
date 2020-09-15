using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RN_TaskManager.Models
{
    [Table("ProjectTaskPerformers")]
    public class ProjectTaskPerformer
    {
        [Key]
        public int ProjectTaskPerformerId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        [ForeignKey("ProjectTask")]
        public int ProjectTaskId { get; set; }
        public ProjectTask Task { get; set; }

        public bool Deleted { get; set; }

    }
}
