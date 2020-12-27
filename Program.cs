using System;

namespace ServiceBusConsole
{
    class Program
    {
        private static void Main()
        {
            Console.WriteLine("Choose the option:");
            Console.WriteLine("1 - Send one message to the queue");
            Console.WriteLine("2 - Send a batch of messages to the queue");
            Console.WriteLine("3 - Receive the messages from to the queue");

            var option = Console.ReadKey();

            Console.WriteLine("");
            Console.WriteLine(" ... working ... ... ");
            Console.WriteLine("");

            switch (option.KeyChar)
            {
                case '1':
                    AzureMessagingService.SendMessageAsync().GetAwaiter().GetResult();
                    break;
                case '2':
                    AzureMessagingService.SendMessageBatchAsync().GetAwaiter().GetResult();
                    break;
                case '3':
                    AzureMessagingService.ReceiveMessagesAsync().GetAwaiter().GetResult();
                    break;
                default:
                    break;
            }

            Console.ReadKey();
            Console.Clear();
            Main();
        }
    }
}
