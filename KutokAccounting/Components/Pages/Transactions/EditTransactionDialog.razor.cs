using KutokAccounting.Components.Pages.Transactions.Models;
using KutokAccounting.Components.Pages.TransactionTypes.Models;
using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.Transactions.Interfaces;
using KutokAccounting.Services.Transactions.Models;
using KutokAccounting.Services.TransactionTypes.Interfaces;
using KutokAccounting.Services.TransactionTypes.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace KutokAccounting.Components.Pages.Transactions;

public partial class EditTransactionDialog : ComponentBase
{
	private bool _isSuccess;

	private TransactionTypeView _transactionType;

	[Inject]
	public required ITransactionService TransactionService { get; set; }

	[Inject]
	public required ITransactionTypeService TransactionTypeService { get; set; }

	[CascadingParameter]
	public IMudDialogInstance MudDialog { get; set; }

	[Parameter]
	public required TransactionView Transaction { get; set; }

	private async Task EditAsync()
	{
		using CancellationTokenSource tokenSource = new(TimeSpan.FromSeconds(30));

		TransactionDto transaction = new()
		{
			Id = Transaction.Id,
			Name = Transaction.Name,
			Description = Transaction.Description,
			Value = Transaction.Money.Value,
			StoreId = Transaction.StoreId,
			TransactionTypeId = Transaction.TransactionType.Id
		};

		await TransactionService.UpdateAsync(transaction, tokenSource.Token);

		MudDialog.Close(DialogResult.Ok(true));
	}

	private async Task<IEnumerable<TransactionTypeView>> SearchAsync(string value, CancellationToken cancellationToken)
	{
		using CancellationTokenSource tokenSource = new(TimeSpan.FromSeconds(30));

		TransactionTypeQueryParameters parameters = new(
			new Services.TransactionTypes.Models.Filters(), value, new Pagination
			{
				Page = 1,
				PageSize = 10
			});

		PagedResult<TransactionType> pagedResult =
			await TransactionTypeService.GetAsync(parameters, tokenSource.Token);

		List<TransactionTypeView> view = pagedResult.Items
			.Select(tp => new TransactionTypeView
			{
				Id = tp.Id,
				Name = tp.Name,
				IsIncome = tp.IsIncome
			}).ToList();

		return view;
	}

	private void OnMoneyChanged(string value)
	{
		try
		{
			Transaction.Money = Money.Parse(value);
		}
		catch (Exception e)
		{
			Transaction.Money = default;
		}
	}

	private string ValidateValue(string value)
	{
		return MoneyFormatRegex.MoneyValueRegex().IsMatch(value)
			? string.Empty
			: "Значення повинно бути додатним числом з двома знаками після точки (наприклад, 123.45 або 123,45).";
	}

	private void Cancel()
	{
		MudDialog.Close();
	}
}