namespace TaskTrack.Repo.Entities;

public class Tag
{
    public int TagID { get; set; }
    public string TagName { get; set; } = null!;
    public string? Color { get; set; }

    public ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
}
