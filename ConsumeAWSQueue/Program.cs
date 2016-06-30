using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Amazon.Runtime;
using CommandLine;
using CommandLine.Text;


namespace ConsumeAWAQueue
{
    class Program
    {

        class Options
        {
            [Option('a', "accessKey", Required = true,
              HelpText = "Amazon AWS credential AccessKey")]
            public string accessKey { get; set; }

            [Option('s', "secretKey", Required = true,
              HelpText = "Amazon AWS credential SecretKey")]
            public string secretKey { get; set; }

            [Option('u', "serviceURL", Required = false,
             HelpText = "Amazon AWS SQS Access Point URL")]
            public string serviceURL { get; set; }

            [Option('q', "queueURL", Required = true,
            HelpText = "Amazon AWS SQS Queue URL")]
            public string queueURL { get; set; }
        }

        
        static void Main(string[] args)
        {

            var result = CommandLine.Parser.Default.ParseArguments<Options>(args);

            if (result.Value.accessKey == null ||
                result.Value.secretKey == null ||
                result.Value.queueURL == null)
            {
                Console.WriteLine("Quitting... missing required parameter.");
                Environment.Exit(1);
            }

            string accessKey = result.Value.accessKey.Trim(); 
            string secretKey = result.Value.secretKey.Trim(); 
            string serviceURL = result.Value.serviceURL; 
            string queueURL = result.Value.queueURL.Trim(); 
            
            if (serviceURL == null)
                serviceURL = "http://sqs.sa-east-1.amazonaws.com";
            else
                serviceURL = serviceURL.Trim();

            var ct = 0;
            
            try
            {
                var awsCredentials = new BasicAWSCredentials(accessKey ,secretKey);

                var sqsConfig = new AmazonSQSConfig();
                sqsConfig.ServiceURL = serviceURL;

                var sqsClient = new AmazonSQSClient(awsCredentials, sqsConfig);

                var receiveMessageRequest = new ReceiveMessageRequest();
                receiveMessageRequest.QueueUrl = queueURL;
                receiveMessageRequest.MaxNumberOfMessages = 10;
          

                var receiveMessageResponse = sqsClient.ReceiveMessage(receiveMessageRequest);
                var messages = receiveMessageResponse.Messages;

                
                Console.WriteLine("Pooling for messages ...");

            
                while (messages.Count != 0)
                {
                    for (int i = 0; i < messages.Count; i++)
                    {
                        ct++;
                        Console.WriteLine("Saving : " + messages[i].MessageId + ".json");
                        File.WriteAllText(messages[i].MessageId + ".json", messages[i].Body);
                        deleteMessageFromQueue(sqsClient, queueURL, messages[i].ReceiptHandle);
                    }
                    receiveMessageResponse = sqsClient.ReceiveMessage(receiveMessageRequest);
                    messages = receiveMessageResponse.Messages;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error retrieving messages: " + e.Message);
                Environment.Exit(2);
            }
            
            if (ct == 0) Console.WriteLine("No messages in queue.");
            Console.WriteLine("Finished pooling messages.");
            Environment.Exit(0);
        }
        static void deleteMessageFromQueue(AmazonSQSClient sqsClient, string queueUrl, string recieptHandle)
        {
            var deleteMessageRequest = new DeleteMessageRequest();
            deleteMessageRequest.QueueUrl = queueUrl;
            deleteMessageRequest.ReceiptHandle = recieptHandle;
            sqsClient.DeleteMessage(deleteMessageRequest);
        }
    }
}
