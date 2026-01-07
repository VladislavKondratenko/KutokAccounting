using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.Stores.Models;
using KutokAccounting.Services.Transactions.Interfaces;
using KutokAccounting.Services.Transactions.Models;
using Microsoft.AspNetCore.Components;

namespace KutokAccounting.Components.Pages.Transactions;

public partial class CalculationResultView : ComponentBase, IDisposable
{
	private CalculationResult? _result = new();

	[Parameter]
	public DateTimeRange Range { get; set; }

	[Parameter]
	public int StoreId { get; set; }

	[Inject]
	public TransactionsStateNotifier TransactionsStateNotifier { get; set; }

	[Inject]
	public required ITransactionService TransactionService { get; set; }

	public void Dispose()
	{
		TransactionsStateNotifier.OnTransactionsChangedAsync -= RecalculateResultAsync;
	}

	protected override async Task OnParametersSetAsync()
	{
		await RecalculateResultAsync();
	}

	protected override void OnInitialized()
	{
		TransactionsStateNotifier.OnTransactionsChangedAsync += RecalculateResultAsync;
	}

	private async Task RecalculateResultAsync()
	{
		using CancellationTokenSource tokenSource = new(TimeSpan.FromSeconds(30));

		_result = new CalculationResult();

		_result.PropertyChanged += (_, _) => StateHasChanged();

		CalculationQueryParameters parameters = new()
		{
			Range = Range,
			StoreId = StoreId
		};

		await TransactionService.CalculateAsync(_result, parameters, tokenSource.Token);

		await InvokeAsync(StateHasChanged);
	}
}