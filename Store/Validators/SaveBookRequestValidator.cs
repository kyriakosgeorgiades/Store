using FluentValidation;
using Store.Models.Books.Request;


public class SaveBookRequestValidator : AbstractValidator<SaveBookRequest>
{
    public SaveBookRequestValidator()
    {
        RuleFor(x => x.BookName)
            .NotEmpty().WithMessage("Book name is required.")
            .Length(3, 255).WithMessage("Book name should be between 3 and 255 characters.");

        RuleFor(x => x.Author.AuthorName)
            .NotEmpty().WithMessage("Author Name is required");

        RuleFor(x => x.ISBN)
            .NotEmpty().WithMessage("ISBN is required.");

        RuleFor(x => x.PublicationYear)
            .NotEmpty().WithMessage("Publication year is required.");
    }
}
