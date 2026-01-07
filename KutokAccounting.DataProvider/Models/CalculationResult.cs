using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KutokAccounting.DataProvider.Models;

public record CalculationResult : INotifyPropertyChanged
{
	private Money _profit;

	public Money Profit
	{
		get => _profit;
		set
		{
			_profit = value;
			OnPropertyChanged();
		}
	}

	public Money Income { get; set; }
	public Money Expense { get; set; }

	public event PropertyChangedEventHandler? PropertyChanged;

	protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}