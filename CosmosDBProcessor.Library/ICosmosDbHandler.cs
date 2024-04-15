using Microsoft.Azure.Cosmos;

namespace CosmosDBProcessor.Library;

public interface ICosmosDbHandler
{
    public CosmosClient Client { get; }
    public Database? Database { get; }
    public Container? Container { get; }

    public Task UnitOfWork(IEnumerable<IContainerItem> items, string workType);
    public Task Create(IContainerItem item);
    public Task<ItemResponse<IContainerItem>> Read(IContainerItem item);
    public Task Delete(IContainerItem item);
    public Task Upsert(IContainerItem item);

}