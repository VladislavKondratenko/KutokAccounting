using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace KutokAccounting.DataProvider.Configurations;

public class DateTimeConverter : ValueConverter<DateTime, long>
{
    public DateTimeConverter() : 
        base(
            ca => ca.ToUniversalTime().Ticks, 
            ca => new DateTime(ca, DateTimeKind.Utc ))
    {
        
    }
}