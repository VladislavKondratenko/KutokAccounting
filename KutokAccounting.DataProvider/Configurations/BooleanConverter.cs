using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace KutokAccounting.DataProvider.Configurations;

public class BooleanConverter : ValueConverter<bool, int>
{
    public BooleanConverter() : base(io => io ? 1 : 0, io => io == 1)
    {
    }
}