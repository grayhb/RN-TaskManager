using AutoMapper;
using RN_TaskManager.Models;
using RN_TaskManager.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RN_TaskManager.Web.AutoMapperProfiles
{
    public class TaskManagerAutoMapperProfile : Profile
    {
        public TaskManagerAutoMapperProfile()
        {
            CreateMap<ProjectTask, ProjectTaskViewModel>()
                .ForMember(e => e.Users, map => map.MapFrom(projectTask => string.Join(",", projectTask.ProjectTaskPerformers.Where(e => !e.Deleted).Select(e => e.UserId))))
                .ForMember(e => e.Performers, map => map.MapFrom(projectTask => string.Join(", ", projectTask.ProjectTaskPerformers.Where(e => !e.Deleted).Select(e => e.User.ShortName))));

            CreateMap<ProjectTaskViewModel, ProjectTask>();
        }
    }
}
