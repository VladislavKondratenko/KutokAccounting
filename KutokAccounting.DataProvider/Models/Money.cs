using System.Globalization;

namespace KutokAccounting.DataProvider.Models;

public readonly record struct Money
{
	public long Value { get; }

	public Money(long value)
	{
		Value = value;
	}

	public static bool operator >(Money left, Money right)
	{
		return left.Value > right.Value;
	}

	public static bool operator <(Money left, Money right)
	{
		return left.Value < right.Value;
	}

	public static Money operator +(Money left, Money right)
	{
		return new Money(left.Value + right.Value);
	}

	public static Money operator -(Money left, Money right)
	{
		return new Money(left.Value - right.Value);
	}

	public static implicit operator long(Money money)
	{
		return money.Value;
	}

	public static Money Parse(string value)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(value);

		ReadOnlySpan<char> span = value.AsSpan();

		Span<char> formattedSpan = stackalloc char[span.Length];

		for (int i = 0; i < span.Length; i++)
		{
			if (span[i] == '.')
			{
				formattedSpan[i] = ',';

				continue;
			}

			formattedSpan[i] = span[i];
		}

		if (decimal.TryParse(formattedSpan, out decimal result))
		{
			return new Money((long) (result * 100));
		}

		throw new FormatException("Invalid money format.");
	}

	public decimal GetValueInCoins()
	{
		return decimal.Divide(Value, 100);
	}

	public override string ToString()
	{
		decimal decimalValue = decimal.Divide(Value, 100);

		return decimalValue.ToString("C2", CultureInfo.GetCultureInfo("uk-UA"));
	}

	public string ToString(string format)
	{
		return GetValueInCoins().ToString(format);
	}
}