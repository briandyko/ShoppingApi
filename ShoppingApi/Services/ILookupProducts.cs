using ShoppingApi.Models.Products;
using System.Threading.Tasks;

namespace ShoppingApi
{
    public interface ILookupProducts
    {
        Task<GetProductsResponse> GetSummary();
        Task<GetProductsListSummary> GetSummaryList(string category);
        Task<GetProductDetailsResponse> GetById(int id);
    }
}