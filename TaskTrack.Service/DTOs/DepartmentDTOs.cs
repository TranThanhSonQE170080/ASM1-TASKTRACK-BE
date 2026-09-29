using System.ComponentModel.DataAnnotations;

namespace TaskTrack.Service.DTOs;

public class DepartmentDTO
{
    public int DepartmentID { get; set; }
    public string DepartmentName { get; set; } = null!;
    public string? DepartmentDescription { get; set; }
    public bool IsActive { get; set; }
}

public class DepartmentCreateDTO
{
    [Required(ErrorMessage = "DepartmentName is required")]
    [StringLength(100, ErrorMessage = "DepartmentName max length is 100")]
    public string DepartmentName { get; set; } = null!;
    [StringLength(300, ErrorMessage = "DepartmentDescription max length is 300")]
    public string? DepartmentDescription { get; set; }
    public bool IsActive { get; set; } = true;
}

public class DepartmentUpdateDTO : DepartmentCreateDTO { }

public class DepartmentDetailDTO : DepartmentDTO
{
    public List<ProjectDTO> Projects { get; set; } = new();
}
