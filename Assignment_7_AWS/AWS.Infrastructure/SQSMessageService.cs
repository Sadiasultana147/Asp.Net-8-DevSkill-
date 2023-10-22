using Amazon.SQS;
using Amazon.SQS.Model;
using AWS.Application;
using AWS.Domain;

namespace AWS.Infrastructure
{
    public class SQSMessageService : ISQSMessageService
    {
        private readonly IAmazonSQS _sqsClient;

        private readonly string queueUrl = "https://sqs.us-east-1.amazonaws.com/847888492411/Sadia-aspnet-b8";

        public SQSMessageService(IAmazonSQS sqsClient)
        {
            _sqsClient = sqsClient;
        }
        public SQSMessageService()
        {
            _sqsClient = new AmazonSQSClient();
        }
        public async Task SendMessageAsync(string messageBody)
        {
            var sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = queueUrl,
                MessageBody = messageBody
            };

            await _sqsClient.SendMessageAsync(sendMessageRequest);
        }
        public async Task<List<Domain.SQSMessage>> GetMessagesFromQueueAsync()
        {
            var receiveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
                MaxNumberOfMessages = 10, // Adjust as needed
                WaitTimeSeconds = 5,
                AttributeNames = new List<string> { "ApproximateReceiveCount" },
            };

            var receiveMessageResponse = await _sqsClient.ReceiveMessageAsync(receiveMessageRequest);

            var allMessages = receiveMessageResponse.Messages.Select(m => new Domain.SQSMessage
            {
                Content = m.Body,
                ReceiptHandle = m.ReceiptHandle
            }).ToList();

            return allMessages;
        }

        public async Task DeleteMessageAsync(string receiptHandle)
        {
            var deleteMessageRequest = new DeleteMessageRequest
            {
                QueueUrl = queueUrl,
                ReceiptHandle = receiptHandle
            };

            await _sqsClient.DeleteMessageAsync(deleteMessageRequest);
        }

        public async Task<int> GetMessageCountAsync()
        {
            var attributes = await _sqsClient.GetQueueAttributesAsync(queueUrl, new List<string> { "ApproximateNumberOfMessages" });
            if (attributes.Attributes.TryGetValue("ApproximateNumberOfMessages", out var messageCount))
            {
                if (int.TryParse(messageCount, out var count))
                {
                    return count;
                }
            }
            return 0; 
        }

    }
}