namespace KutokAccounting.Services.Vendors.DataTransferObjects;

public sealed record QueryParameters(string? Name, string? SearchString, int Page = 1, int PageSize = 10);