namespace TaskTrack.Repo.Entities;

public class Project
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = null!;
    public string? Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public short Status { get; set; }            // SMALLINT: 0 NotStarted, 1 InProgress, 2 Completed, 3 OnHold
    public int DepartmentID { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public Department Department { get; set; } = null!;
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
