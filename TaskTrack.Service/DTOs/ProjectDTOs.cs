using System.ComponentModel.DataAnnotations;

namespace TaskTrack.Service.DTOs;

public class ProjectDTO
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = null!;
    public string? Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public int Status { get; set; }
    public int DepartmentID { get; set; }
    public string? DepartmentName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class ProjectDetailDTO : ProjectDTO
{
    public List<TaskDTO> Tasks { get; set; } = new();
}

public class ProjectCreateDTO
{
    [Required(ErrorMessage = "ProjectName is required")]
    [StringLength(200, ErrorMessage = "ProjectName max length is 200")]
    public string ProjectName { get; set; } = null!;
    public string? Description { get; set; }
    [Required(ErrorMessage = "StartDate is required")]
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    [Range(0, 3, ErrorMessage = "Status must be 0-3")]
    public int Status { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "DepartmentID must be greater than 0")]
    public int DepartmentID { get; set; }
    public bool IsActive { get; set; } = true;
}

public class ProjectUpdateDTO : ProjectCreateDTO { }

public class ProjectSearchDTO
{
    public string? Name { get; set; }
    public int? Status { get; set; }
    public int? DepartmentId { get; set; }
}
