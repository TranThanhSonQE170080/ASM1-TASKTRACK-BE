using TaskTrack.Repo.Data;
using TaskTrack.Repo.Entities;

namespace TaskTrack.Repo.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly TaskManagementContext _context;
    public IGenericRepository<Department> Departments { get; }
    public IGenericRepository<Project> Projects { get; }
    public IGenericRepository<TaskItem> Tasks { get; }
    public IGenericRepository<Tag> Tags { get; }
    public IGenericRepository<TaskTag> TaskTags { get; }

    public UnitOfWork(TaskManagementContext context)
    {
        _context = context;
        Departments = new GenericRepository<Department>(context);
        Projects = new GenericRepository<Project>(context);
        Tasks = new GenericRepository<TaskItem>(context);
        Tags = new GenericRepository<Tag>(context);
        TaskTags = new GenericRepository<TaskTag>(context);
    }

    public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
