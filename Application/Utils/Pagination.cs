using Application.ServiceResponse;

namespace Application.Utils
{
    public class Pagination
    {
        public static async Task<PaginationModel<T>> GetPagination<T>(IEnumerable<T> enumerable, int page, int pageSize)
        {
            var startIndex = (page - 1) * pageSize;
            var currentPageData = enumerable.Skip(startIndex).Take(pageSize).ToList();
            await Task.Delay(1);
            var totalRecords = enumerable.Count();

            var paginationModel = new PaginationModel<T>
            {
                Page = page,
                TotalRecords = totalRecords,
                TotalPage = (int)Math.Ceiling(totalRecords / (double)pageSize),
                ListData = currentPageData
            };

            return paginationModel;
        }
    }
}
