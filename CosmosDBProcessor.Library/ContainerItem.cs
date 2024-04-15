namespace CosmosDBProcessor.Library;

public record ContainerItem(
#pragma warning disable IDE1006 // Naming Styles
string id,
string name
#pragma warning restore IDE1006 // Naming Styles
) : IContainerItem
{
    string IContainerItem.Name => name;
    string IContainerItem.ID => id;
}