using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestfulApiProject.DTOs;
using RestfulApiProject.Services;
using System.Security.Claims;

namespace RestfulApiProject.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (currentUserIdClaim == null || !int.TryParse(currentUserIdClaim.Value, out int currentUserId))
            return Unauthorized();

        var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

        // Users can only view their own profile unless they're Admin or Manager
        if (currentUserId != id && currentUserRole != "Admin" && currentUserRole != "Manager")
            return Forbid();

        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (currentUserIdClaim == null || !int.TryParse(currentUserIdClaim.Value, out int currentUserId))
            return Unauthorized();

        var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

        // Managers can only update their own profile or regular users
        // They cannot update other managers or admins
        if (currentUserRole == "Manager")
        {
            var targetUser = await _userService.GetUserByIdAsync(id);
            if (targetUser == null)
                return NotFound();

            // Manager can update themselves or users with User role
            if (currentUserId != id && targetUser.Role != Models.UserRole.User)
                return Forbid("Managers can only update their own profile or regular users");

            // Managers cannot assign Admin or Manager roles
            if (request.Role.HasValue && request.Role.Value != Models.UserRole.User)
                return Forbid("Managers cannot assign Admin or Manager roles");
        }

        var updatedUser = await _userService.UpdateUserAsync(id, request, currentUserId);
        if (updatedUser == null)
            return Conflict(new { message = "Username or email already exists, or user not found" });

        return Ok(updatedUser);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        var result = await _userService.DeleteUserAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}