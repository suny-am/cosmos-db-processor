using Azure.Identity;
using Microsoft.Azure.Cosmos;

namespace CosmosDBProcessor.Library;

public class CosmosDbHandler : ICosmosDbHandler
{
    private readonly CosmosClient _client = new(
        accountEndpoint: Environment.GetEnvironmentVariable("COSMOS_ENDPOINT")!,
        tokenCredential: new DefaultAzureCredential()
        );
    private Database _database = null!;
    private Container _container = null!;

    public CosmosClient Client => _client;
    public Database Database => _database;
    public Container Container => _container;

    public async Task LoadDataSource(string databaseID, string containerID, string partitionKeyPath)
    {
        _database = await _client.CreateDatabaseIfNotExistsAsync(databaseID);
        _container = await _database.CreateContainerIfNotExistsAsync(
            id: containerID,
            partitionKeyPath: partitionKeyPath
        );
    }

    public async Task UnitOfWork(IEnumerable<IContainerItem> items, string workType)
    {
        foreach (var item in items)
        {
            switch (workType)
            {
                case "CREATE":
                    {
                        await Create(item);
                        break;
                    }
                case "READ":
                    {
                        await Read(item);
                        break;
                    }
                case "DELETE":
                    {
                        await Delete(item);
                        break;
                    }
                case "UPSERT":
                    {
                        await Upsert(item);
                        break;
                    }
            }
        }
    }

    public async Task Upsert(IContainerItem item)
    {
        await _container.UpsertItemAsync(item, new PartitionKey(item.Name));
    }

    public async Task Create(IContainerItem item)
    {
        await _container.CreateItemAsync(item, new PartitionKey(item.Name));
    }

    public async Task<ItemResponse<IContainerItem>> Read(IContainerItem searchItem)
    {
        ItemResponse<IContainerItem> item = await _container.ReadItemAsync<IContainerItem>(searchItem.ID, new PartitionKey(searchItem.Name));
        return item;
    }

    public async Task Delete(IContainerItem record)
    {
        await _container.DeleteItemAsync<IContainerItem>(record.Name, new PartitionKey(record.Name));
    }
}

