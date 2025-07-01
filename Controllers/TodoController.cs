using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Protection de tout le contrôleur
public class TodoController : ControllerBase
{
    private readonly TodoContext _context;

    public TodoController(TodoContext context)
    {
        _context = context;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            throw new UnauthorizedAccessException("Utilisateur non authentifié");
        }
        return userId;
    }

    [HttpGet]
    public ActionResult<IEnumerable<TodoItem>> GetAll()
    {
        var userId = GetCurrentUserId();
        var todos = _context.Todos
            .Where(t => t.UserId == userId)
            .ToList(); // Pas besoin d'inclure User
        
        return Ok(todos);
    }

    [HttpGet("{id}")]
    public ActionResult<TodoItem> GetById(int id)
    {
        var userId = GetCurrentUserId();
        var item = _context.Todos
            .FirstOrDefault(t => t.Id == id && t.UserId == userId); // Pas besoin d'inclure User
        
        if (item == null)
        {
            return NotFound(new ErrorResponse
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "Élément non trouvé",
                Status = 404,
                Detail = $"L'élément Todo avec l'ID {id} n'a pas été trouvé.",
                TraceId = HttpContext.TraceIdentifier
            });
        }
        
        return Ok(item);
    }

    [HttpPost]
    public ActionResult<TodoItem> Create(CreateTodoRequest request)
    {
        if (request == null)
            return BadRequest(new ErrorResponse
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Données manquantes",
                Status = 400,
                Detail = "Les données de la requête ne peuvent pas être null.",
                TraceId = HttpContext.TraceIdentifier
            });

        var userId = GetCurrentUserId();
        
        // Créer un nouvel item avec l'ID utilisateur
        var newItem = new TodoItem
        {
            Title = request.Title,
            IsDone = request.IsDone,
            UserId = userId
        };

        _context.Todos.Add(newItem);
        _context.SaveChanges();
        
        return CreatedAtAction(nameof(GetById), new { id = newItem.Id }, newItem);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, UpdateTodoRequest request)
    {
        if (request == null)
            return BadRequest(new ErrorResponse
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Données manquantes",
                Status = 400,
                Detail = "Les données de mise à jour ne peuvent pas être null.",
                TraceId = HttpContext.TraceIdentifier
            });

        var userId = GetCurrentUserId();
        var existing = _context.Todos.FirstOrDefault(t => t.Id == id && t.UserId == userId);
        
        if (existing == null) 
        {
            return NotFound(new ErrorResponse
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "Élément non trouvé",
                Status = 404,
                Detail = $"L'élément Todo avec l'ID {id} n'a pas été trouvé.",
                TraceId = HttpContext.TraceIdentifier
            });
        }

        existing.Title = request.Title;
        existing.IsDone = request.IsDone;
        _context.SaveChanges();
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var userId = GetCurrentUserId();
        var item = _context.Todos.FirstOrDefault(t => t.Id == id && t.UserId == userId);
        
        if (item == null) 
        {
            return NotFound(new ErrorResponse
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "Élément non trouvé",
                Status = 404,
                Detail = $"L'élément Todo avec l'ID {id} n'a pas été trouvé.",
                TraceId = HttpContext.TraceIdentifier
            });
        }

        _context.Todos.Remove(item);
        _context.SaveChanges();
        
        return NoContent();
    }
}
