namespace TaskTrack.Repo.Entities;

public class TaskItem
{
    public int TaskID { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    // SMALLINT in the official schema => short in EF Core (Npgsql requires exact type mapping)
    public short Status { get; set; }            // 0 ToDo, 1 InProgress, 2 Done, 3 Cancelled
    public short Priority { get; set; }          // 0 Low, 1 Medium, 2 High, 3 Critical
    public DateOnly? DueDate { get; set; }
    public int ProjectID { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedDate { get; set; }

    public Project Project { get; set; } = null!;
    public ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
}
