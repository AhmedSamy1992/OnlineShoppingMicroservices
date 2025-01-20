using BuildingBlocks.CQRS;
using Catalog.Api.Models;

namespace Catalog.Api.Products.CreateProduct
{
    public record CreateProductCommand(string Name, string Description, string ImageFile, decimal Price, List<string> Category)
            :ICommand<CreateProductResult>;

    public record CreateProductResult(Guid Id);
    public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, CreateProductResult>
    {

        public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Name = command.Name,
                Description = command.Description,
                ImageFile = command.ImageFile,
                Price = command.Price,
                Category = command.Category
            };

            return new CreateProductResult(Guid.NewGuid());
        }
    }
}

