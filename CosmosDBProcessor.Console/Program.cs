using System.Diagnostics;
using CosmosDBProcessor.Library;
var stopWatch = new Stopwatch();

Console.WriteLine("beginning timed tests...");
stopWatch.Start();
CosmosDbHandler handler = new();
stopWatch.Stop();
Console.WriteLine("Handler creation time in Milliseconds: " + stopWatch.ElapsedMilliseconds); ;

string? databaseID = Environment.GetEnvironmentVariable("DATABASE_ID");
string? containerID = Environment.GetEnvironmentVariable("CONTAINER_ID");
string? partitionKeyPath = Environment.GetEnvironmentVariable("PARTITION_KEY_PATH");

if (databaseID is null || containerID is null || partitionKeyPath is null)
{
    throw new Exception("could not load required environment variables");
}

stopWatch.Restart();
try
{
    await handler.LoadDataSource(databaseID, containerID, partitionKeyPath);
}
catch (Exception ex)
{

    throw new Exception(ex.Message);
}
stopWatch.Stop();
Console.WriteLine("Data source load time in Milliseconds: " + stopWatch.ElapsedMilliseconds); ;

stopWatch.Restart();
var items = await handler.AllItems();

if (items is null)
{
    Console.WriteLine("no items found");
}
Console.WriteLine("Item count from response: " + items!.Count);
stopWatch.Stop();
Console.WriteLine("Item read time in Milliseconds: " + stopWatch.ElapsedMilliseconds);