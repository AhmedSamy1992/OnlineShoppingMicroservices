using BuildingBlocks.CQRS;
using Catalog.Api.Models;
using Marten;
using Microsoft.Extensions.Logging;

namespace Catalog.Api.Products.GetProductByCategory
{
    public record GetProductByCategoryQuery(string category) : IQuery<GetProductByCategoryResult>;

    public record GetProductByCategoryResult(IEnumerable<Product> Products);
    internal class GetProductByCategoryQueryHandler(IDocumentSession session, ILogger<GetProductByCategoryQueryHandler> logger)
        : IQueryHandler<GetProductByCategoryQuery, GetProductByCategoryResult>
    {
        public async Task<GetProductByCategoryResult> Handle(GetProductByCategoryQuery query, CancellationToken cancellationToken)
        {
            logger.LogInformation("get product by category");

            var products = await session.Query<Product>().Where(p => p.Category.Contains(query.category)).ToListAsync();
            return new GetProductByCategoryResult(products);
        }
    }
}
