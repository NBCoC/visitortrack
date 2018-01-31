using System;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using VisitorTrack.Entities;

namespace VisitorTrack.Database
{
    public abstract class BaseManager : IDisposable
    {
        protected readonly DocumentClient DocumentClient;

        protected BaseManager(string databaseName, string endpointUri, string accountKey)
        {
            DatabaseName = databaseName;
            EndpointUri = endpointUri;
            AccountKey = accountKey;

            DocumentClient = new DocumentClient(new Uri(endpointUri), accountKey);
        }

        protected string DatabaseName { get; }

        protected string EndpointUri { get; }

        protected string AccountKey { get; }

        public void Dispose() => DocumentClient?.Dispose();

        protected abstract string CollectionName { get; }

        public async Task CreateCollectionIfNotExistsAsync()
        {
            await DocumentClient.CreateDatabaseIfNotExistsAsync(
                new Microsoft.Azure.Documents.Database() { Id = DatabaseName });

            await DocumentClient.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(DatabaseName), new DocumentCollection() { Id = CollectionName });
        }

        public async Task CreateAsync(object entity)
            =>  await DocumentClient.CreateDocumentAsync(
                    UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName), entity);

        public async Task DeleteAsync(IEntity entity)
            => await DocumentClient.DeleteDocumentAsync(
                    UriFactory.CreateDocumentUri(DatabaseName, CollectionName, entity.Id));

        public async Task UpdateAsync(string id, object entity)
            =>  await DocumentClient.ReplaceDocumentAsync(
                    UriFactory.CreateDocumentUri(DatabaseName, CollectionName, id), entity);
    }
}