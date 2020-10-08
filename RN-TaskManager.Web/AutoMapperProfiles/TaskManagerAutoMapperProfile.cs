using AutoMapper;
using RN_TaskManager.Models;
using RN_TaskManager.Web.ViewModels;
using System.Linq;

namespace RN_TaskManager.Web.AutoMapperProfiles
{
    public class TaskManagerAutoMapperProfile : Profile
    {
        public TaskManagerAutoMapperProfile()
        {
            CreateMap<ProjectTask, ProjectTaskViewModel>()
                .ForMember(e => e.Note, map => map.MapFrom(projectTask => string.IsNullOrEmpty(projectTask.Note) ? "" : projectTask.Note))
                .ForMember(e => e.EffectBeforeHours, map => map.MapFrom(projectTask => projectTask.EffectBeforeHours == null ? 0 : projectTask.EffectBeforeHours))
                .ForMember(e => e.EffectAfterHours, map => map.MapFrom(projectTask => projectTask.EffectAfterHours == null ? 0 : projectTask.EffectAfterHours))
                .ForMember(e => e.BlockId, map => map.MapFrom(projectTask => projectTask.BlockId == null ? 0 : projectTask.BlockId))
                .ForMember(e => e.TaskTypeId, map => map.MapFrom(projectTask => projectTask.TaskTypeId == null ? 0 : projectTask.TaskTypeId))
                .ForMember(e => e.Users, map => map.MapFrom(projectTask => string.Join(",", projectTask.ProjectTaskPerformers.Where(e =>!e.Deleted).Select(e => e.UserId))))
                .ForMember(e => e.PerformerIds, map => map.MapFrom(projectTask => projectTask.ProjectTaskPerformers.Where(e =>!e.Deleted).Select(e => e.UserId)))
                .ForMember(e => e.Performers, map => map.MapFrom(projectTask => string.Join(", ", projectTask.ProjectTaskPerformers.Where(e => !e.Deleted).Select(e => e.User.ShortName))));

            CreateMap<ProjectTaskViewModel, ProjectTask>();
        }
    }
}