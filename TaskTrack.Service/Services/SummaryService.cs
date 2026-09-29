using TaskTrack.Repo.Repositories;
using TaskTrack.Service.DTOs;

namespace TaskTrack.Service.Services;

public class SummaryService : ISummaryService
{
    private readonly IUnitOfWork _uow;

    public SummaryService(IUnitOfWork uow) => _uow = uow;

    public async Task<SummaryDTO> GetSummaryAsync()
    {
        var depts = (await _uow.Departments.GetAllAsync()).Count(d => d.IsActive);
        var projects = (await _uow.Projects.GetAllAsync()).Count(p => p.IsActive);
        var tasks = (await _uow.Tasks.GetAllAsync()).Count(t => t.IsActive);
        return new SummaryDTO { DepartmentCount = depts, ProjectCount = projects, TaskCount = tasks };
    }
}
