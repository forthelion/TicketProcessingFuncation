using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.SQS.Model;
using Amazon.SQS;
using Newtonsoft.Json;
using Amazon.JSII.JsonModel.Spec;
AWSSDK.SimpleNotificationService;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace InsuranceDataFunction;

public class Function
{
    /// <summary>
    /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
    /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
    /// region the Lambda function is executed in.
    /// </summary>
    private readonly IAmazonSQS sqsClient;
    public Function()
    {
        sqsClient = new AmazonSQSClient();
    }


    /// <summary>
    /// This method is called for every Lambda invocation. This method takes in an SQS event object and can be used 
    /// to respond to SQS messages.
    /// </summary>
    /// <param name="evnt"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
    {
        if (evnt?.Records == null)
        {
            context.Logger.LogInformation($"nothing to do null event");
            return;
        }
        foreach (var message in evnt.Records)
        {
            await ProcessMessageAsync(message, context);
            await DeleteMessage(message, context);
        }
    }
    public class Vehicle
    {
        public string Color { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
    }

    public class Data
    {
        public string owner { get; set; }
        public string ownerContact { get; set; }
        public Vehicle Vehicle { get; set; }
        public string LicensePlate { get; set; }
        public string Date { get; set; }
        public string ViolationAddress { get; set; }
        public string ViolationType { get; set; }
        public string TicketAmount { get; set; }
    }

    private async Task ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
    {
        context.Logger.LogInformation($"Processed message {message.Body}");

        string json = message.Body;


        // Deserialize the JSON into the Data object
        Data data = JsonConvert.DeserializeObject<Data>(json);

        // Access the extracted values
        string ownerName = data.owner;
        string ownerContact = data.ownerContact;
        string color = data.Vehicle.Color;
        string make = data.Vehicle.Make;
        string model = data.Vehicle.Model;
        string licensePlate = data.LicensePlate;
        string date = data.Date;
        string location = data.ViolationAddress;
        string typeOfViolation = data.ViolationType;
        string ticketPrice = data.TicketAmount;

        // Use the extracted values as needed
        Console.WriteLine($"Owner: {ownerName}");
        Console.WriteLine($"Owner Contact: {ownerContact}");
        Console.WriteLine($"Color: {color}");
        Console.WriteLine($"Make: {make}");
        Console.WriteLine($"Model: {model}");
        Console.WriteLine($"License Plate: {licensePlate}");
        Console.WriteLine($"Date: {date}");
        Console.WriteLine($"Violation Address: {location}");
        Console.WriteLine($"Violation Type: {typeOfViolation}");
        Console.WriteLine($"Ticket Amount: {ticketPrice}");







        await Task.CompletedTask;
    }
    //private static async Task DeleteMessage(IAmazonSQS sqsClient, Message message, string qUrl,ILambdaContext context)
    private async Task DeleteMessage(SQSEvent.SQSMessage message, ILambdaContext context)
    {
        context.Logger.LogInformation($"\nDeleting message {message.MessageId} from queue...");
        await sqsClient.DeleteMessageAsync(new DeleteMessageRequest { QueueUrl = "https://sqs.us-east-1.amazonaws.com/392106205903/UpwardQueue", ReceiptHandle = message.ReceiptHandle });
    }
}