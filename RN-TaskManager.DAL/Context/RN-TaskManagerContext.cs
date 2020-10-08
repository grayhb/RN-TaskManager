using Microsoft.EntityFrameworkCore;
using RN_TaskManager.Models;

namespace RN_TaskManager.DAL.Context
{
    public class RN_TaskManagerContext : DbContext
    {
        public RN_TaskManagerContext(DbContextOptions<RN_TaskManagerContext> options) : base(options) { }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<ProjectTask> ProjectTasks { get; set; }
        public virtual DbSet<ProjectTaskStatus> ProjectTaskStatuses { get; set; }
        public virtual DbSet<ProjectTaskType> ProjectTaskTypes { get; set; }
        public virtual DbSet<ProjectTaskPerformer> ProjectTaskPerformers { get; set; }
        public virtual DbSet<TaskType> TaskTypes { get; set; }
        public virtual DbSet<Block> Blocks { get; set; }

    }
}
