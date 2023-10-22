using Amazon.DynamoDBv2.Model;
using AWS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS.Application
{
    public interface IDynamoDbService
    {
        Task AddBookAsync(Book book);
        Task<List<Dictionary<string, AttributeValue>>> GetBookListAsync();
        Task<Book> GetBookByIdAsync(string pk);

        Task EditBookAsync(Book book);
        Task DeleteBookAsync(int bookId);
    }

}
