using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TodoApi.Models;

public class TodoItem
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Le titre est obligatoire")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Le titre doit contenir entre 1 et 200 caractères")]
    public string Title { get; set; } = "";
    
    public bool IsDone { get; set; }
    
    // Relation avec User
    public int UserId { get; set; }
    
    [JsonIgnore] // Éviter la sérialisation pour éviter les cycles
    public User User { get; set; } = null!;
}
