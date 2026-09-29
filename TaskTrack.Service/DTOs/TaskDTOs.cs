using System.ComponentModel.DataAnnotations;

namespace TaskTrack.Service.DTOs;

public class TagDTO
{
    public int TagID { get; set; }
    public string TagName { get; set; } = null!;
    public string? Color { get; set; }
}

public class TaskDTO
{
    public int TaskID { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int Status { get; set; }
    public int Priority { get; set; }
    public DateOnly? DueDate { get; set; }
    public int ProjectID { get; set; }
    public string? ProjectName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public List<TagDTO> Tags { get; set; } = new();
}

public class TaskCreateDTO
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(300, ErrorMessage = "Title max length is 300")]
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    [Range(0, 3, ErrorMessage = "Status must be 0-3")]
    public int Status { get; set; }
    [Range(0, 3, ErrorMessage = "Priority must be 0-3")]
    public int Priority { get; set; }
    public DateOnly? DueDate { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "ProjectID must be greater than 0")]
    public int ProjectID { get; set; }
    public List<int>? TagIDs { get; set; }
}

public class TaskUpdateDTO : TaskCreateDTO { }

public class TaskSearchDTO
{
    public string? Title { get; set; }
    public int? Status { get; set; }
    public int? Priority { get; set; }
    public int? ProjectId { get; set; }
    public int? TagId { get; set; }
}
