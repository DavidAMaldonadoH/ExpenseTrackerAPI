using FluentValidation;

namespace ExpenseTrackerAPI.Models;

public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public List<Expense> Expenses = [];
}

public class CreateCategoryRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}

public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Description).MaximumLength(255);
    }
}

public class UpdateCategoryRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}

public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Description).MaximumLength(255);
    }
}

public class GetCategoryResponse
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}