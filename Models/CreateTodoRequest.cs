using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models;

public class CreateTodoRequest
{
    [Required(ErrorMessage = "Le titre est obligatoire")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Le titre doit contenir entre 1 et 200 caractères")]
    public string Title { get; set; } = "";
    
    public bool IsDone { get; set; } = false;
} 