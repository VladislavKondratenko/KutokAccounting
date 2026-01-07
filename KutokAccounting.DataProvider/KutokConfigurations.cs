namespace KutokAccounting.DataProvider;

public static class KutokConfigurations
{
	public const string CustomTransactionTypeCode = "CUSTOM";
	public const string OpenInvoiceTransactionTypeCode = "OPEN_INVOICE";
	public const string CloseInvoiceTransactionTypeCode = "CLOSE_INVOICE";
	public const string WriteOperationsSemaphore = "WriteOperationsSemaphore";

	public static readonly string ConnectionString =
		$"Data Source={Path.Combine(AppContext.BaseDirectory, "KutokData.db")}";
}