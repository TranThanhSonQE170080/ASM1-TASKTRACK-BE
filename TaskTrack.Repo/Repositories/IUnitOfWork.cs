using TaskTrack.Repo.Entities;

namespace TaskTrack.Repo.Repositories;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Department> Departments { get; }
    IGenericRepository<Project> Projects { get; }
    IGenericRepository<TaskItem> Tasks { get; }
    IGenericRepository<Tag> Tags { get; }
    IGenericRepository<TaskTag> TaskTags { get; }
    Task<int> SaveChangesAsync();
}
