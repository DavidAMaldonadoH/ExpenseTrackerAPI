using ExpenseTrackerAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerAPI.Controllers;

public class CategoriesController(AppDbContext dbContext) : BaseController
{
    private readonly AppDbContext _dbContext = dbContext;

    /// <summary>
    /// Get all categories
    /// </summary>
    /// <returns>An array with all the available categories</returns>
    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetCategoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _dbContext.Categories.ToArrayAsync();
        return Ok(categories.Select(CategoryToGetCategoryResponse));
    }

    /// <summary>
    /// Get a category by its id
    /// </summary>
    /// <param name="id">The ID of the category</param>
    /// <returns>The single category record</returns>
    [Authorize]
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GetCategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var category = await _dbContext.Categories.SingleOrDefaultAsync(c => c.Id == id);
        if (category == null)
        {
            return NotFound();
        }

        return Ok(CategoryToGetCategoryResponse(category));
    }

    /// <summary>
    /// Create a new category
    /// </summary>
    /// <param name="category">The category to be created</param>
    /// <returns>A link to the category that was created</returns>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(GetCategoryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest category)
    {
        var newCategory = new Category
        {
            Name = category.Name,
            Description = category.Description
        };

        _dbContext.Categories.Add(newCategory);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = newCategory.Id }, newCategory);
    }

    /// <summary>
    /// Update a category
    /// </summary>
    /// <param name="id">The ID of the category to update</param>
    /// <param name="category">The category data to update</param>
    /// <returns>The category updated</returns>
    [Authorize]
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(GetCategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCategoryRequest category)
    {
        var existingCategory = await _dbContext.Categories.FindAsync(id);
        if (existingCategory == null)
        {
            return NotFound();
        }

        existingCategory.Name = category.Name;
        existingCategory.Description = category.Description;

        try
        {
            _dbContext.Entry(existingCategory).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return Ok(existingCategory);
        }
        catch
        {
            return StatusCode(500, "An error has occurred while updating the category");
        }
    }

    /// <summary>
    /// Delete a category
    /// </summary>
    /// <param name="id">The ID of the category to delete</param>
    /// <returns></returns>
    [Authorize]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var category = await _dbContext.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        _dbContext.Categories.Remove(category);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    private static GetCategoryResponse CategoryToGetCategoryResponse(Category category)
    {
        return new GetCategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }
}