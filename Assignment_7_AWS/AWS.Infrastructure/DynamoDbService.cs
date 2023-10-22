using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using AWS.Application;
using AWS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using System.Text.Json;
using Amazon.DynamoDBv2.DocumentModel;

namespace AWS.Infrastructure
{
    public class DynamoDbService : IDynamoDbService
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly string tableName = "Sadia-Booktbl"; 

        public DynamoDbService(IAmazonDynamoDB dynamoDbClient)
        {
            _dynamoDbClient = dynamoDbClient;
        }

        public async Task AddBookAsync(Book book)
        {
            var request = new PutItemRequest
            {
                TableName = tableName,
                Item = new Dictionary<string, AttributeValue>
            {
                { "BookId", new AttributeValue { N = book.BookId.ToString() } },
                { "Title", new AttributeValue { S = book.Title } },
                { "Author", new AttributeValue { S = book.Author } }
                
            }
            };

            await _dynamoDbClient.PutItemAsync(request);
        }

        public async Task<List<Dictionary<string, AttributeValue>>> GetBookListAsync()
        {
            var request = new ScanRequest
            {
                TableName = tableName,
                Limit = 10, 
            };

            var response = await _dynamoDbClient.ScanAsync(request);

            if (response.Items.Count > 0)
            {
                return response.Items;
            }
            else
            {
                
                return new List<Dictionary<string, AttributeValue>>();
            }
        }
        public async Task<Book> GetBookByIdAsync(string pk)
        {
            var getItemRequest = new GetItemRequest
            {
                TableName = tableName,
                Key = new Dictionary<string, AttributeValue>
        {
            { "BookId", new AttributeValue { N = pk.ToString() } }
        }
            };

            var response = await _dynamoDbClient.GetItemAsync(getItemRequest);

            if (response.Item.Count == 0)
            {
                return null;
            }

            var itemAsDocument = Document.FromAttributeMap(response.Item);
            var result = JsonSerializer.Deserialize<Book>(itemAsDocument.ToJson());
            return result;
        }

        public async Task EditBookAsync(Book book)
        {
            Table table = Table.LoadTable(_dynamoDbClient, tableName);
            var document = new Document
            {
                ["BookId"] = book.BookId,
                ["Title"] = book.Title,
                ["Author"] = book.Author
                
            };
            await table.UpdateItemAsync(document);
        }

        public async Task DeleteBookAsync(int bookId)
        {
            var request = new DeleteItemRequest
            {
                TableName = tableName,
                Key = new Dictionary<string, AttributeValue>
        {
            { "BookId", new AttributeValue { N = bookId.ToString() } }
        }
            };

            await _dynamoDbClient.DeleteItemAsync(request);
        }

    }




}
