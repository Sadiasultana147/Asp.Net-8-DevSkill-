using AWS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS.Application
{
    public interface ISQSMessageService
    {
        Task SendMessageAsync(string messageBody);
        Task<List<SQSMessage>> GetMessagesFromQueueAsync();
        Task DeleteMessageAsync(string receiptHandle);
        Task<int> GetMessageCountAsync();
    }
}
