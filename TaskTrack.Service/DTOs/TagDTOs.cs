using System.ComponentModel.DataAnnotations;

namespace TaskTrack.Service.DTOs;

public class TagCreateDTO
{
    [Required(ErrorMessage = "TagName is required")]
    [StringLength(50, ErrorMessage = "TagName max length is 50")]
    public string TagName { get; set; } = null!;
    public string? Color { get; set; }
}

public class TagUpdateDTO : TagCreateDTO { }
