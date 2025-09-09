namespace KutokAccounting.Services.Vendors.DataTransferObjects;

public sealed record QueryParameters(string? Name, int Page = 1, int PageSize = 10);
