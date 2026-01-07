using KutokAccounting.Services.TransactionTypes.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace KutokAccounting.Components.Pages.TransactionTypes;

public partial class AddTransactionTypeDialog
{
	private string? _name;

	private bool _isIncome;

	private bool _isSuccess;

	[CascadingParameter]
	public IMudDialogInstance MudDialog { get; set; }

	private async Task AddAsync()
	{
		using CancellationTokenSource tokenSource = new(TimeSpan.FromSeconds(30));

		TransactionTypeDto transactionType = new()
		{
			Name = _name,
			IsIncome = _isIncome
		};

		await TransactionTypeService.CreateAsync(transactionType, tokenSource.Token);

		MudDialog.Close(DialogResult.Ok(true));
	}

	private void Cancel()
	{
		MudDialog.Close();
	}
}