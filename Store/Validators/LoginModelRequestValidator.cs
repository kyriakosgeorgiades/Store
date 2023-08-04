using FluentValidation;
using Store.Models.Users.Request;

public class LoginModelRequestValidator : AbstractValidator<LoginModelRequest>
{
    public LoginModelRequestValidator()
    {
        RuleFor(x => x.UserNameOrEmail)
            .NotEmpty().WithMessage("Username or Email is required.")
            .MaximumLength(255).WithMessage("Username or Email should not exceed 255 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Length(6, 100).WithMessage("Password should be between 6 and 100 characters.");
    }
}
