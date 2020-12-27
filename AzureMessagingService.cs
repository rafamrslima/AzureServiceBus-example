using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceBusConsole
{
    public static class AzureMessagingService
    {
        static readonly string _connectionString = "connectionString-gotFromAzurePortalAfterCreatingTheQueue";
        static readonly string _queueName = "queue-name";

        public static async Task SendMessageAsync()
        {
            await using var client = new ServiceBusClient(_connectionString);
            ServiceBusSender sender = client.CreateSender(_queueName);

            var message = new ServiceBusMessage("Hello world!");

            await sender.SendMessageAsync(message);
            Console.WriteLine($"Sent a single message to the queue: {_queueName}");
        }

        public static async Task SendMessageBatchAsync()
        {
            await using var client = new ServiceBusClient(_connectionString);
            ServiceBusSender sender = client.CreateSender(_queueName);
            Queue<ServiceBusMessage> messages = CreateMessages();

            int messageCount = messages.Count;

            while (messages.Count > 0)
            {
                using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

                while (messages.Count > 0 && messageBatch.TryAddMessage(messages.Peek()))
                    messages.Dequeue();

                await sender.SendMessagesAsync(messageBatch);

                // if there are any remaining messages in the queue, the while loop repeats 
            }

            Console.WriteLine($"Sent a batch of {messageCount} messages to the queue: {_queueName}");
        }

        static Queue<ServiceBusMessage> CreateMessages()
        {
            var messages = new Queue<ServiceBusMessage>();
            messages.Enqueue(new ServiceBusMessage("First message in the batch"));
            messages.Enqueue(new ServiceBusMessage("Second message in the batch"));
            messages.Enqueue(new ServiceBusMessage("Third message in the batch"));
            return messages;
        }

        public static async Task ReceiveMessagesAsync()
        {
            await using var client = new ServiceBusClient(_connectionString);
            ServiceBusProcessor processor = client.CreateProcessor(_queueName, new ServiceBusProcessorOptions());

            processor.ProcessMessageAsync += MessageHandler;
            processor.ProcessErrorAsync += ErrorHandler;

            await processor.StartProcessingAsync();
            Console.ReadKey();            
            await processor.StopProcessingAsync();
        }

        static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine($"Received: {body}");

            // message is deleted from the queue after received. 
            await args.CompleteMessageAsync(args.Message);
        }

        static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
