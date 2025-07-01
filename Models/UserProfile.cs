using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Le prénom est obligatoire")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Le prénom doit contenir entre 2 et 50 caractères")]
        public string FirstName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Le nom de famille est obligatoire")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Le nom de famille doit contenir entre 2 et 50 caractères")]
        public string LastName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "L'email est obligatoire")]
        [EmailAddress(ErrorMessage = "Le format de l'email n'est pas valide")]
        [StringLength(100, ErrorMessage = "L'email ne peut pas dépasser 100 caractères")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La date de naissance est obligatoire")]
        [DataType(DataType.Date)]
        [PastDate(ErrorMessage = "La date de naissance doit être dans le passé")]
        public DateTime BirthDate { get; set; }
    }

    // Validation personnalisée pour vérifier les dates dans le passé
    public class PastDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime date)
            {
                return date < DateTime.Now;
            }
            return false;
        }
    }
}
