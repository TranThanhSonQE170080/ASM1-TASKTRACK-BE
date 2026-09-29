namespace TaskTrack.Repo.Entities;

public class Department
{
    public int DepartmentID { get; set; }
    public string DepartmentName { get; set; } = null!;
    public string? DepartmentDescription { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Project> Projects { get; set; } = new List<Project>();
}
