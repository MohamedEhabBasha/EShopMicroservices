namespace Catalog.API.Products.CreateProduct;

public record CreateProductCommand(string Name, List<string> Category, string Description, string ImageFile, decimal Price): ICommand<CreateProductResult>;
public record CreateProductResult(Guid Id);

// subscription to validator behavior pipline by inheriting from AbstractValidator as it inherit from IValidator
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Category).NotEmpty().WithMessage("Category is required");
            RuleFor(x => x.ImageFile).NotEmpty().WithMessage("ImageFile is required");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than 0");
    }
}
internal class CreateProductCommandHandler(IDocumentSession session, ILogger<CreateProductCommandHandler> logger) 
    : ICommandHandler<CreateProductCommand, CreateProductResult>
{
    public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("CreateProductCommandHandler.Handle called with {@Command}", command);
        //Pre validation
        #region OldWay
        //var result = await validator.ValidateAsync(command, cancellationToken);

        //var errors = result.Errors.Select(x => x.ErrorMessage).ToList();

        //if (errors.Any())
        //{
        //    throw new ValidationException(errors.FirstOrDefault());
        //} 
        #endregion

        // Business Logic to create a product
        // 1- Create product entity from command object
        var product = new Product
        {
            Name = command.Name,
            Category = command.Category,
            Description = command.Description,
            ImageFile = command.ImageFile,
            Price = command.Price
        };
        // 2- save to database
        session.Store(product);
        await session.SaveChangesAsync(cancellationToken);
        // 3- return CreateProductResult result
        return new CreateProductResult(product.Id);
    }
}
