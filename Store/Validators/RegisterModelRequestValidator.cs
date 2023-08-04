using FluentValidation;
using Store.Models.Users.Request;

public class RegisterModelRequestValidator : AbstractValidator<RegisterModelRequest>
{
    public RegisterModelRequestValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username is required.")
            .Length(3, 255).WithMessage("Username should be between 3 and 255 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MaximumLength(255).WithMessage("Email should not exceed 255 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Length(6, 100).WithMessage("Password should be between 6 and 100 characters.");
    }
}
