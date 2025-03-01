using ExpenseTrackerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerAPI.Controllers;
public class UsersController(AppDbContext dbContext) : BaseController
{
    private readonly AppDbContext _dbContext = dbContext;

    /// <summary>
    /// Get all users
    /// </summary>
    /// <returns>An array with all the registered users</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        var users = await _dbContext.Users.ToArrayAsync();
        return Ok(users.Select(UserToGetUserResponse));
    }

    /// <summary>
    /// Get a user by its id
    /// </summary>
    /// <param name="id">The ID of the user</param>
    /// <returns>The single user record</returns>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(GetUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById([FromRoute] long id)
    {
        var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        var userResponse = UserToGetUserResponse(user);
        return Ok(userResponse);
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="user">The user to be created</param>
    /// <returns>A link to the user that was created</returns>
    [HttpPost]
    [ProducesResponseType(typeof(GetUserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest user)
    {
        user.Password = PasswordHelper.HashPassword(user.Password);

        var newUser = new User
        {
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Password = user.Password
        };
        _dbContext.Users.Add(newUser);
        await _dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = newUser.Id }, newUser);
    }

    /// <summary>
    /// Update a user
    /// </summary>
    /// <param name="id">The ID of the user to update</param>
    /// <param name="user">The user data to update</param>
    /// <returns>The updated user</returns>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(GetUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateUserRequest user)
    {
        var existingUser = await _dbContext.Users.FindAsync(id);
        if (existingUser == null)
        {
            return NotFound();
        }

        existingUser.FirstName = user.FirstName;
        existingUser.LastName = user.LastName;
        existingUser.Password = user.Password;

        try
        {
            _dbContext.Entry(existingUser).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();

            return Ok(existingUser);
        }
        catch
        {
            return StatusCode(500, "An error occurred while updating the user");
        }
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    /// <param name="id">The ID of the user to be deleted</param>
    /// <returns></returns>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(long id)
    {
        var user = await _dbContext.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
        return NoContent();
    }

    private static GetUserResponse UserToGetUserResponse(User user)
    {
        return new GetUserResponse
        {
            Id = user.Id,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
        };
    }
}