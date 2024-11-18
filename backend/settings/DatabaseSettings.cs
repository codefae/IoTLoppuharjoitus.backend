namespace backend.settings;

public class DatabaseSettings<T>
{
    public required string ConnectionString { get; init; }
    public required string DatabaseName { get; init; }
    public required string CollectionName { get; init; }
}

