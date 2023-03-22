using CI_Platform_Entity.Models;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace CI_Platform.Models
{
    public class PaginationModel
    {
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }

        public PaginationModel()
        {

        }
        public PaginationModel(int totalItems, int page, int pageSize = 3)
        {
            int totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
            int currentPage = page;

            int startPage = currentPage - 3;
            int endPage = currentPage + 3;

            if (startPage <= 0)
            {
                endPage = endPage - (-startPage - 1);
                startPage = 1;
            }


            if (endPage > totalPages)
            {
                endPage = totalPages;
                //if (endPage < 10)
                //{
                // startPage = endPage - 9;
                //}
            }

            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;



        }

    }
}