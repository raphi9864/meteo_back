using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserProfilesController : ControllerBase
{
    private readonly TodoContext _context;

    public UserProfilesController(TodoContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserProfile>>> GetUserProfiles()
    {
        return await _context.UserProfiles.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserProfile>> GetUserProfile(int id)
    {
        var profile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.Id == id);
        if (profile == null)
            throw new KeyNotFoundException($"Le profil utilisateur avec l'ID {id} n'a pas été trouvé");

        return Ok(profile);
    }

    [HttpPost]
    public async Task<ActionResult<UserProfile>> CreateUserProfile(UserProfile profile)
    {
        if (profile == null)
            throw new ArgumentNullException(nameof(profile), "Le profil utilisateur ne peut pas être null");

        _context.UserProfiles.Add(profile);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUserProfile), new { id = profile.Id }, profile);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUserProfile(int id, UserProfile profile)
    {
        if (profile == null)
            throw new ArgumentNullException(nameof(profile), "Les données de mise à jour ne peuvent pas être null");

        var existing = await _context.UserProfiles.FindAsync(id);
        if (existing == null) 
            throw new KeyNotFoundException($"Le profil utilisateur avec l'ID {id} n'a pas été trouvé");

        existing.FirstName = profile.FirstName;
        existing.LastName = profile.LastName;
        existing.Email = profile.Email;
        existing.BirthDate = profile.BirthDate;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUserProfile(int id)
    {
        var profile = await _context.UserProfiles.FindAsync(id);
        if (profile == null) 
            throw new KeyNotFoundException($"Le profil utilisateur avec l'ID {id} n'a pas été trouvé");

        _context.UserProfiles.Remove(profile);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
