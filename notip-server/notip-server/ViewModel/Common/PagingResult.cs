namespace notip_server.ViewModel.Common
{
    public class PagingResult<T>
    {
        public PagingResult(List<T>? items, int pageIndex, int pageSize, int totalRecords, int totalPage)
        {
            Items = items;

            PageIndex = pageIndex;

            PageSize = pageSize;

            TotalRecords = totalRecords;

            TotalPage = totalPage;
        }

        //public PagingResult(List<T>? items, int pageIndex, int pageSize, string? sortBy, string? orderBy, int totalRecords, int totalPages)
        //{
        //    Items = items;

        //    PageIndex = pageIndex;

        //    PageSize = pageSize;

        //    SortBy = sortBy;

        //    OrderBy = orderBy;

        //    TotalRecords = totalRecords;

        //    TotalPages = totalPages;
        //}

        public List<T>? Items { set; get; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        //public string? SortBy { get; set; }

        //public string? OrderBy { get; set; }

        public int TotalRecords { get; set; }

        public int TotalPage { get; set; }
    }
}
