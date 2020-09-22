using RN_TaskManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RN_TaskManager.Web.ViewModels
{
    public class ProjectTaskViewModel
    {
        public int ProjectTaskId { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public int? ProjectTaskTypeId { get; set; }
        public ProjectTaskType TaskType { get; set; }

        public string Details { get; set; }

        public int Priority { get; set; }
        public int DurationHours { get; set; }

        public DateTime? StartPlan { get; set; }
        public DateTime? EndPlan { get; set; }

        public DateTime? StartFact { get; set; }
        public DateTime? EndFact { get; set; }


        public int? ProjectTaskStatusId { get; set; }
        public ProjectTaskStatus TaskStatus { get; set; }


        public int? GroupId { get; set; }
        public Group Group { get; set; }

        public bool Deleted { get; set; }

        public string Users { get; set; }
        public string Performers { get; set; }


        public string ProjectName => Project != null ? Project.ProjectName : "";
        public int ProjectImportance => Project != null ? Project.ProjectImportance : 0;


        public string GroupName => Group != null ? Group.GroupName : "";

        public string TaskTypeName => TaskType != null ? TaskType.ProjectTaskTypeName : "";

        public string TaskStatusName => TaskStatus != null ? TaskStatus.StatusName : "";
    }
}
