namespace MovieAPI.Helpers
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; set; }

        public bool HasPrevious => PageIndex > 1;
        public bool HasNext => PageIndex < TotalPages;

        public PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            PageIndex = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            AddRange(items);
        }

        public static PaginatedList<T> ToPageList(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize)
                              .Take(pageSize)
                              .ToList();

            return new PaginatedList<T>(items, count, pageNumber, pageSize);
        }
        public static PaginatedList<T> ToPageList(List<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize)
                              .Take(pageSize)
                              .ToList();

            return new PaginatedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
