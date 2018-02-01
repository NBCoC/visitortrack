using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using VisitorTrack.EntityManager.Models;

namespace VisitorTrack.EntityManager
{
    public abstract class BaseManager : IDisposable
    {
        protected readonly DocumentClient DocumentClient;

        protected BaseManager(string databaseId, string endpointUrl, string accountKey)
        {
            if (string.IsNullOrEmpty(databaseId))
                throw new ArgumentNullException(nameof(databaseId));

            if (string.IsNullOrEmpty(endpointUrl))
                throw new ArgumentNullException(nameof(endpointUrl));

            if (string.IsNullOrEmpty(accountKey))
                throw new ArgumentNullException(nameof(accountKey));

            DatabaseId = databaseId;

            DocumentClient = new DocumentClient(new Uri(endpointUrl), accountKey);
        }

        protected string DatabaseId { get; }

        public void Dispose() => DocumentClient?.Dispose();

        protected abstract string CollectionId { get; }

        protected Uri GetCollectionUri() => UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId);

        protected string CanonicalizeEmail(string email) => email.ToLower().Trim();

        public async Task CreateIfNotExistsAsync()
        {
            await DocumentClient.CreateDatabaseIfNotExistsAsync(new Database() { Id = DatabaseId });

            await DocumentClient.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(DatabaseId), new DocumentCollection() { Id = CollectionId });
        }

        protected async Task<string> CreateEntityAsync(object entity)
            => (await DocumentClient.CreateDocumentAsync(
                    UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), entity)).Resource.Id;

        public async Task DeleteEntityAsync(string entityId)
            => await DocumentClient.DeleteDocumentAsync(
                    UriFactory.CreateDocumentUri(DatabaseId, CollectionId, entityId));

        public async Task UpdateEntityAsync(string id, object entity)
            => await DocumentClient.ReplaceDocumentAsync(
                    UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id), entity);

    }
}