using FluentValidation;

namespace ExpenseTrackerAPI.Models;
public class User
{
    public long Id { get; set; }
    public required string Username { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Password { get; set; }
    public List<Expense> Expenses { get; set; } = [];
}

public class CreateUserRequest
{
    public required string Username { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Password { get; set; }
}

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MaximumLength(64);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(64);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}

public class UpdateUserRequest
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Password { get; set; }
}

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(64);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}

public class GetUserResponse
{
    public long Id { get; set; }
    public required string Username { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}