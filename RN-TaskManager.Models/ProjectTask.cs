using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RN_TaskManager.Models
{
    [Table("ProjectTasks")]
    public class ProjectTask
    {
        [Key]
        public int ProjectTaskId { get; set; }

        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        [ForeignKey("ProjectTaskType")]
        public int? ProjectTaskTypeId { get; set; }
        public ProjectTaskType TaskType { get; set; }

        public string Details { get; set; }

        public int Priority { get; set; }
        public int DurationHours { get; set; }

        public DateTime? StartPlan { get; set; }
        public DateTime? EndPlan { get; set; }

        public DateTime? StartFact { get; set; }
        public DateTime? EndFact { get; set; }
        

        [ForeignKey("ProjectTaskStatus")]
        public int? ProjectTaskStatusId { get; set; }
        public ProjectTaskStatus TaskStatus { get; set; }


        [ForeignKey("Group")]
        public int? GroupId { get; set; }
        public Group Group { get; set; }

        public bool Deleted { get; set; }


        public DateTime? DateCreated { get; set; }
        public DateTime? DateEdited { get; set; }
        public DateTime? DateDeleted { get; set; }

        public string LoginCreated { get; set; }
        public string LoginEdited { get; set; }
        public string LoginDeleted { get; set; }

        public List<ProjectTaskPerformer> ProjectTaskPerformers { get; set; }
    }
}
