namespace KutokAccounting.Services.Vendors.DataTransferObjects;

public sealed record Pagination
{
    public int Page { get; set; }
    public int PageSize { get; set; }

    public int Skip => (Page - 1) * PageSize;
}
