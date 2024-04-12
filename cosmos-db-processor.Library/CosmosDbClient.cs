using Azure.Identity;
using GithubAPI.Library.GraphQL.Records;
using Microsoft.Azure.Cosmos;

namespace GithubAPI.Library.Azure.Cosmos;

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

    public async Task LoadDatabase()
    {
        _database = await _client.CreateDatabaseIfNotExistsAsync("gh-api-data");
        _container = await _database.CreateContainerIfNotExistsAsync(
            id: "Repositories",
            partitionKeyPath: "/name"
        );
    }

    public async Task UnitOfWork(IEnumerable<Repository> repositories, string workType)
    {
        foreach (var repository in repositories)
        {
            switch (workType)
            {
                case "CREATE":
                    {
                        await Create(repository);
                        break;
                    }
                case "READ":
                    {
                        await Read(repository);
                        break;
                    }
                case "DELETE":
                    {
                        await Delete(repository);
                        break;
                    }
                case "UPSERT":
                    {
                        await Upsert(repository);
                        break;
                    }
            }
        }
    }

    public async Task Upsert(Repository record)
    {
        await _container.UpsertItemAsync(record, new PartitionKey(record.name));
    }

    public async Task Create(Repository record)
    {
        await _container.CreateItemAsync(record, new PartitionKey(record.name));
    }

    public async Task<Repository> Read(Repository record)
    {
        ItemResponse<Repository> repository = await _container.ReadItemAsync<Repository>(record.id, new PartitionKey(record.name));
        return repository;
    }

    public async Task Delete(Repository record)
    {
        await _container.DeleteItemAsync<Repository>(record.name, new PartitionKey(record.name));
    }
}

