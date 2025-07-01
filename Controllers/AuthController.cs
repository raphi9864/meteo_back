using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;
using TodoApi.Services;
using BCrypt.Net;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly TodoContext _context;
    private readonly IJwtService _jwtService;

    public AuthController(TodoContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        // Vérifier si l'utilisateur existe déjà
        var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
        if (existingUser != null)
        {
            return BadRequest(new ErrorResponse
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Erreur d'inscription",
                Status = 400,
                Detail = "Un utilisateur avec cet email existe déjà.",
                TraceId = HttpContext.TraceIdentifier
            });
        }

        // Créer le nouvel utilisateur
        var user = new User
        {
            Email = request.Email.ToLower().Trim(),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Générer le token JWT
        var token = _jwtService.GenerateToken(user);
        var expiresAt = DateTime.UtcNow.AddDays(7);

        return Ok(new AuthResponse
        {
            Token = token,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ExpiresAt = expiresAt
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        // Trouver l'utilisateur
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email.ToLower().Trim());
        
        if (user == null || !user.IsActive)
        {
            return BadRequest(new ErrorResponse
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Erreur de connexion",
                Status = 400,
                Detail = "Email ou mot de passe incorrect.",
                TraceId = HttpContext.TraceIdentifier
            });
        }

        // Vérifier le mot de passe
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return BadRequest(new ErrorResponse
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Erreur de connexion",
                Status = 400,
                Detail = "Email ou mot de passe incorrect.",
                TraceId = HttpContext.TraceIdentifier
            });
        }

        // Générer le token JWT
        var token = _jwtService.GenerateToken(user);
        var expiresAt = DateTime.UtcNow.AddDays(7);

        return Ok(new AuthResponse
        {
            Token = token,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ExpiresAt = expiresAt
        });
    }

    [HttpGet("me")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<ActionResult<User>> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        
        if (user == null)
        {
            return NotFound();
        }

        // Ne pas retourner le mot de passe
        user.PasswordHash = string.Empty;
        
        return Ok(user);
    }
} 