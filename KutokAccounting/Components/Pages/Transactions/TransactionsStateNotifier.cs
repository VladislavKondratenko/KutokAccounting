namespace KutokAccounting.Components.Pages.Transactions;

public class TransactionsStateNotifier
{
	public async Task TransactionsChangedAsync()
	{
		if (OnTransactionsChangedAsync is not null)
		{
			await OnTransactionsChangedAsync.Invoke();
		}
	}

	public event Func<Task>? OnTransactionsChangedAsync;
}