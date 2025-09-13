namespace KutokAccounting;

public static class KutokConfigurations
{
    public static readonly string ConnectionString =
        $"Data Source={Path.Combine(AppContext.BaseDirectory, "KutokData.db")}";
}