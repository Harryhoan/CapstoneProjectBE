namespace Application.ServiceResponse
{
    public class ServiceResponse<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; } = true;

        public string? Message { get; set; } = null;

        public string? Error { get; set; } = null;

        //public string? Hint { get; set; } = null;

        //public double? PriceTotal { get; set; } = null;

        public List<string>? ErrorMessages { get; set; } = null;
    }
    public class PaginationModel<T>
    {
        public int Page { get; set; }
        public int TotalPage { get; set; }
        public int TotalRecords { get; set; }
        public IEnumerable<T> ListData { get; set; } = new List<T>();
    }
}
