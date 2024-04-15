using CosmosDBProcessor.Library;

CosmosDbHandler handler = new();

string? databaseID = Environment.GetEnvironmentVariable("DATABASE_ID");
string? containerID = Environment.GetEnvironmentVariable("CONTAINER_ID");
string? partitionKeyPath = Environment.GetEnvironmentVariable("PARTITION_KEY_PATH");

if (databaseID is null || containerID is null || partitionKeyPath is null)
{
    throw new Exception("could not load required environment variables");
}

try
{
    await handler.LoadDataSource(databaseID, containerID, partitionKeyPath);
}
catch (Exception ex)
{

    throw new Exception(ex.Message);
}

var items = await handler.AllItems();

if (items is null)
{
    Console.WriteLine("no items found");
}

Console.WriteLine("Item count from response: " + items!.Count);