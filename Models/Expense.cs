using FluentValidation;

namespace ExpenseTrackerAPI.Models;

public class Expense
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public decimal Amount { get; set; }
    public DateOnly PurchaseDate { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public long UserId { get; set; }
    public User User { get; set; } = null!;
}

public class CreateExpenseRequest
{
    public required string Name { get; set; }
    public decimal Amount { get; set; }
    public required string PurchaseDate { get; set; }
    public int CategoryId { get; set; }
}

public class CreateExpenseRequestValidator : AbstractValidator<CreateExpenseRequest>
{
    public CreateExpenseRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.PurchaseDate).NotEmpty();
        RuleFor(x => x.CategoryId).GreaterThan(0);
    }
}

public class UpdateExpenseRequest
{
    public required string Name { get; set; }
    public decimal Amount { get; set; }
    public required string PurchaseDate { get; set; }
    public int CategoryId { get; set; }
}

public class UpdateExpenseRequestValidator : AbstractValidator<UpdateExpenseRequest>
{
    public UpdateExpenseRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.PurchaseDate).NotEmpty();
        RuleFor(x => x.CategoryId).GreaterThan(0);
    }
}

public class GetExpenseResponse
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public decimal Amount { get; set; }
    public DateOnly PurchaseDate { get; set; }
    public required string Category { get; set; }
}

public class GetAllExpensesRequest
{
    public int? Page { get; set; }
    public int? RecordsPerPage { get; set; }

    public GetAllExpensesFilter? filter;
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}

public class GetAllExpensesRequestValidator : AbstractValidator<GetAllExpensesRequest>
{
    public GetAllExpensesRequestValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.RecordsPerPage).GreaterThan(0);
        RuleFor(x => x.filter).IsInEnum();
        RuleFor(x => x.StartDate).NotEmpty().When(x => x.filter == GetAllExpensesFilter.Custom);
        RuleFor(x => x.EndDate).NotEmpty().When(x => x.filter == GetAllExpensesFilter.Custom);
        RuleFor(x => x.StartDate).LessThanOrEqualTo(x => x.EndDate).When(x => x.filter == GetAllExpensesFilter.Custom && x.StartDate.HasValue && x.EndDate.HasValue);
    }
}

public enum GetAllExpensesFilter
{
    LastWeek,
    LasMonth,
    LastThreeMonths,
    Custom
}