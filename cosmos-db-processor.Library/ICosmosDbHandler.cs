using Microsoft.Azure.Cosmos;
using GithubAPI.Library.GraphQL.Records;

public interface ICosmosDbHandler
{
    public CosmosClient Client { get; }
    public Database Database { get; }
    public Container Container { get; }

    public Task UnitOfWork(IEnumerable<Repository> repositories, string workType);
    public Task Create(Repository repository);
    public Task<Repository> Read(Repository repository);
    public Task Delete(Repository repository);
    public Task Upsert(Repository repository);

}