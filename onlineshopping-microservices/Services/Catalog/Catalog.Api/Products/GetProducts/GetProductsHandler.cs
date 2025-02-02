using BuildingBlocks.CQRS;
using Catalog.Api.Models;
using Marten;
using Marten.Pagination;

namespace Catalog.Api.Products.GetProducts
{
    public record GetProductsQuery(int? pageNumber = 1, int? pageSize = 10) : IQuery<GetProductsResult>;
    public record GetProductsResult(IEnumerable<Product> Products);
    public class GetProductsQueryHandler(IDocumentSession session) : IQueryHandler<GetProductsQuery, GetProductsResult>
    {
        public async Task<GetProductsResult> Handle(GetProductsQuery query, CancellationToken cancellationToken)
        {
            var products = await session.Query<Product>().ToPagedListAsync(query.pageNumber ?? 1, query.pageSize ?? 10, cancellationToken);

            return new GetProductsResult(products);
        }
    }
}
