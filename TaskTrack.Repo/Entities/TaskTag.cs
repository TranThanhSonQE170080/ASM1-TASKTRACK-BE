namespace TaskTrack.Repo.Entities;

public class TaskTag
{
    public int TaskID { get; set; }
    public int TagID { get; set; }
    public TaskItem Task { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}
