namespace MovieAPI.Models.Pagination
{
    public class Pager
    {
        const int maxPageSize = 50;
        public int pageIndex { get; set; } = 1;
        
        private int _pageSize = 10;
        public int pageSize
        {
            get { return _pageSize; }
            set { _pageSize = (value > maxPageSize) ? maxPageSize : value; }
        }

    }
}
