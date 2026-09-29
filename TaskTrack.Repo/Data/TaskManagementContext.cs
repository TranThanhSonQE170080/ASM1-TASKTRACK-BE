using Microsoft.EntityFrameworkCore;
using TaskTrack.Repo.Entities;

namespace TaskTrack.Repo.Data;

public class TaskManagementContext : DbContext
{
    public TaskManagementContext(DbContextOptions<TaskManagementContext> options) : base(options) { }

    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<TaskTag> TaskTags => Set<TaskTag>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Department>(e =>
        {
            e.ToTable("Department");
            e.HasKey(x => x.DepartmentID);
        });

        mb.Entity<Project>(e =>
        {
            e.ToTable("Project");
            e.HasKey(x => x.ProjectID);
            e.Property(x => x.CreatedDate).HasColumnType("timestamp without time zone");
            e.HasOne(x => x.Department)
             .WithMany(d => d.Projects)
             .HasForeignKey(x => x.DepartmentID)
             .OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<TaskItem>(e =>
        {
            e.ToTable("Task");
            e.HasKey(x => x.TaskID);
            e.Property(x => x.CreatedDate).HasColumnType("timestamp without time zone");
            e.Property(x => x.ModifiedDate).HasColumnType("timestamp without time zone");
            e.HasOne(x => x.Project)
             .WithMany(p => p.Tasks)
             .HasForeignKey(x => x.ProjectID)
             .OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<Tag>(e =>
        {
            e.ToTable("Tag");
            e.HasKey(x => x.TagID);
        });

        mb.Entity<TaskTag>(e =>
        {
            e.ToTable("TaskTag");
            e.HasKey(x => new { x.TaskID, x.TagID });
            e.HasOne(x => x.Task).WithMany(t => t.TaskTags).HasForeignKey(x => x.TaskID);
            e.HasOne(x => x.Tag).WithMany(t => t.TaskTags).HasForeignKey(x => x.TagID);
        });
    }
}
