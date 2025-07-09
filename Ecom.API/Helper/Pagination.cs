using Ecom.Core.DTO;

namespace Ecom.API.Helper
{
    public class Pagination<T> where T : class
    {
        private IEnumerable<ProductDTO> product;

        public Pagination(int pageNumber, int pageSize, int totalCount, IEnumerable<ProductDTO> product)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
            this.product = product;
        }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}
