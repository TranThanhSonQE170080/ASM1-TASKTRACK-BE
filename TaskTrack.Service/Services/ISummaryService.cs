using TaskTrack.Service.DTOs;

namespace TaskTrack.Service.Services;

public interface ISummaryService
{
    Task<SummaryDTO> GetSummaryAsync();
}
