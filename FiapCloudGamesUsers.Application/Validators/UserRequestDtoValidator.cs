using FiapCloudGamesUsers.Application.Dtos;
using FluentValidation;

namespace FiapCloudGamesUsers.Application.Validators;

public class UserRequestDtoValidator : AbstractValidator<UserRequestDto>
{
    public UserRequestDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("User name is required.")
            .MaximumLength(100).WithMessage("Name must have a maximum of 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");
    }
}

public class UserUpdateRequestDtoValidator : AbstractValidator<UserUpdateRequestDto>
{
        public UserUpdateRequestDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("User name is required.")
                .MaximumLength(100).WithMessage("Name must have a maximum of 100 characters.");

            RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one number.")
            .Matches("[!@#$%^&*,.?;:]").WithMessage("Password must contain at least one special character.");
    }
}

public class UserUnlockRequestDtoValidator : AbstractValidator<UserUnlockRequestDto>
{
    public UserUnlockRequestDtoValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one number.")
            .Matches("[!@#$%^&*,.?;:]").WithMessage("Password must contain at least one special character.");
    }
}
