using System.Security.Claims;
using ExpenseTrackerAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerAPI.Controllers;

public class ExpensesController(AppDbContext dbContext) : BaseController
{
    private readonly AppDbContext _dbContext = dbContext;

    /// <summary>
    /// Get all expenses for the authenticated user
    /// </summary>
    /// <param name="request">Filters to the array of expenses</param>
    /// <returns>An array of the filtered expenses</returns>
    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetExpenseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetExpenses([FromQuery] GetAllExpensesRequest request)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "-1");

        int page = request?.Page ?? 1;
        int numberOfRecords = request?.RecordsPerPage ?? 20;

        var existingUser = await _dbContext.Users.FindAsync(userId);
        if (existingUser == null)
        {
            return NotFound();
        }

        IQueryable<Expense> query = _dbContext.Expenses
            .Include(e => e.Category)
            .Skip((page - 1) * numberOfRecords)
            .Take(numberOfRecords)
            .Where(e => e.UserId == userId);

        if (request != null && request.filter != null)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            switch (request.filter)
            {
                case GetAllExpensesFilter.LastWeek:
                    query = query.Where(e => e.PurchaseDate >= today.AddDays(-7));
                    break;
                case GetAllExpensesFilter.LasMonth:
                    query = query.Where(e => e.PurchaseDate >= today.AddMonths(-1));
                    break;
                case GetAllExpensesFilter.LastThreeMonths:
                    query = query.Where(e => e.PurchaseDate >= today.AddMonths(-3));
                    break;
                default:
                    if (request.StartDate != null)
                    {
                        query = query.Where(e => e.PurchaseDate >= request.StartDate);
                    }

                    if (request.EndDate != null)
                    {
                        query = query.Where(e => e.PurchaseDate <= request.EndDate);
                    }
                    break;
            }
        }

        var expenses = await query.ToArrayAsync();

        return Ok(expenses.Select(ExpenseToGetExpenseResponse));
    }

    /// <summary>
    /// Get an expense by its id
    /// </summary>
    /// <param name="id">The ID of the expense</param>
    /// <returns>The single expense record</returns>
    [Authorize]
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(GetExpenseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetExpenseById([FromRoute] long id)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "-1");
        var existingUser = await _dbContext.Users.FindAsync(userId);
        if (existingUser == null)
        {
            return NotFound();
        }
        Console.WriteLine(existingUser.ToString());

        var existingExpense = await _dbContext.Expenses.Include(e => e.Category).SingleOrDefaultAsync(e => e.Id == id);
        if (existingExpense == null)
        {
            return NotFound();
        }

        Console.WriteLine(existingExpense);

        return Ok(ExpenseToGetExpenseResponse(existingExpense));

    }

    /// <summary>
    /// Create a new expense for the authenticated user
    /// </summary>
    /// <param name="expense">The expense to be created</param>
    /// <returns>A link to the expense that was created</returns>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(GetExpenseResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateUserExpense([FromBody] CreateExpenseRequest expense)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "-1");
        var existingUser = await _dbContext.Users.FindAsync(userId);
        if (existingUser == null)
        {
            return NotFound();
        }

        var newExpense = new Expense
        {
            Name = expense.Name,
            Amount = expense.Amount,
            PurchaseDate = DateOnly.ParseExact(expense.PurchaseDate, "MM/dd/yyyy"),
            CategoryId = expense.CategoryId,
            UserId = userId
        };

        _dbContext.Expenses.Add(newExpense);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetExpenseById), new { id = newExpense.Id }, ExpenseToGetExpenseResponse(newExpense));
    }

    /// <summary>
    /// Update an existing expense for the authenticated user
    /// </summary>
    /// <param name="id">The ID of the expense to update</param>
    /// <param name="expense">The expense data to update</param>
    /// <returns>The updated expense</returns>
    [Authorize]
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(GetExpenseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateUserExpense([FromRoute] long id, [FromBody] UpdateExpenseRequest expense)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "-1");
        var existingUser = await _dbContext.Users.FindAsync(userId);
        if (existingUser == null)
        {
            return NotFound();
        }

        var existingExpense = await _dbContext.Expenses.Include(e => e.Category).SingleOrDefaultAsync(e => e.Id == id);
        if (existingExpense == null)
        {
            return NotFound();
        }

        existingExpense.Name = expense.Name;
        existingExpense.Amount = expense.Amount;
        existingExpense.PurchaseDate = DateOnly.ParseExact(expense.PurchaseDate, "MM/dd/yyyy");
        existingExpense.CategoryId = expense.CategoryId;

        try
        {
            _dbContext.Entry(existingExpense).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return Ok(ExpenseToGetExpenseResponse(existingExpense));
        }
        catch
        {
            return StatusCode(500, "An error has occurred while updating the expense");
        }
    }

    /// <summary>
    /// Delete an existing expense for the authenticated user
    /// </summary>
    /// <param name="id">The ID of the expense to delete</param>
    /// <returns></returns>
    [Authorize]
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUserExpense([FromRoute] long id)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "-1");
        var existingUser = await _dbContext.Users.FindAsync(userId);
        if (existingUser == null)
        {
            return NotFound();
        }

        var existingExpense = await _dbContext.Expenses.FindAsync(id);
        if (existingExpense == null)
        {
            return NotFound();
        }

        _dbContext.Remove(existingExpense);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    private static GetExpenseResponse ExpenseToGetExpenseResponse(Expense expense)
    {
        return new GetExpenseResponse
        {
            Id = expense.Id,
            Name = expense.Name,
            Amount = expense.Amount,
            PurchaseDate = expense.PurchaseDate,
            Category = expense.Category.Name
        };
    }
}