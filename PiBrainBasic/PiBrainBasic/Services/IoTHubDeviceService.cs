using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using PiBrainBasic.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Services.SerializationService;

namespace PiBrainBasic.Services
{
  public static class IoTHubDeviceService
  {

    static ISerializationService serializationService;

    static IoTHubDeviceService()
    {
      serializationService = SerializationService.Json;
    }

    public static event EventHandler<PingCommandEventArgs> PingCommandReceived;
    public static event EventHandler<BrewCommandEventArgs> BrewCommandReceived;
    public static event EventHandler<TakePictureCommandEventArgs> TakePictureCommandReceived;
    public static event EventHandler<AnswerCommandEventArgs> AnswerCommandReceived;
    public static event EventHandler<CommandEventArgs> UnknownCommandReceived;
    public static event EventHandler<MessageEventArgs> BadMessageReceived;
    public static event EventHandler<MessageEventArgs> MessageReceived;

    static DeviceClient deviceClient;
    //static string iotHubUri = "udliothub.azure-devices.net";
    //static string deviceId = "udlw10pi";
    //static string deviceKey = "c3CtwPMX6TLAQcdi0Cn+YA3wXWTpy2+Rg9uZN6QkUUo=";
    static string iotHubUri = "marsiot.azure-devices.net";
    static string deviceId = "coffeepot";
    static string deviceKey = "YcsP7DdIFde4s1QKd4xJgxpo0w0ipJo7wybZVWM/p4I=";

    public static async Task StartListening()
    {
      ReceiveCloudToDeviceMessagesAsync();
      await Task.CompletedTask;
    }

    private static void EnsureDeviceClient()
    {
      deviceClient = deviceClient ?? DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey));
    }

    public static async Task SendDeviceToCloudMessagesAsync(object MessageData, string correlationId)
    {
      EnsureDeviceClient();

      var messageString = JsonConvert.SerializeObject(MessageData);
      var message = new Message(Encoding.ASCII.GetBytes(messageString));
      message.CorrelationId = correlationId;
      Debug.WriteLine(String.Format("correlation-id: {0}", correlationId));

      Debug.WriteLine(">>>>> {0} - Sending message: {1}", DateTime.Now, messageString);
      await deviceClient.SendEventAsync(message);
    }

    public static async Task CompleteCloudToDeviceMessageAsync(Message Message)
    {
      EnsureDeviceClient();
      await deviceClient.CompleteAsync(Message);
    }

    private static async void ReceiveCloudToDeviceMessagesAsync()
    {
      Debug.WriteLine("\nReceiving cloud to device messages from service");

      EnsureDeviceClient();

      while (true)
      {
        Message receivedMessage = await deviceClient.ReceiveAsync();
        if (receivedMessage == null) continue;

        var messageBytes = receivedMessage.GetBytes();
        var messageString = Encoding.ASCII.GetString(messageBytes);

        //Fire the general message received event
        NotifyMessageReceived(receivedMessage, messageBytes, messageString);

        //Attempt to parse it into a CommandMessage
        CloudToDeviceMessage commandMessage = null;
        try
        {
          commandMessage = JsonConvert.DeserializeObject<CloudToDeviceMessage>(messageString);
        }
        catch
        {

        }

        //if (serializationService.TryDeserialize<CommandMessage>(messageString, out commandMessage))
        if (commandMessage != null)
        {
          string commandString = commandMessage.Command.Trim().ToLower();
          switch(commandString)
          {
            case "ping":
              string payload = commandMessage.Parameters;
              PingCommandReceived?.Invoke(null, new PingCommandEventArgs(receivedMessage, messageBytes, messageString, commandMessage, payload));
              break;
            case "brew":
              BrewCommandReceived?.Invoke(null, new BrewCommandEventArgs(receivedMessage, messageBytes, messageString, commandMessage));
              break;
            case "takepicture":
              int camera;
              if(int.TryParse(commandMessage.Parameters,out camera))
              {
                TakePictureCommandReceived?.Invoke(null, new TakePictureCommandEventArgs(receivedMessage, messageBytes, messageString, commandMessage, camera));
              }
              else
              {
                NotifyBadMessageReceived(receivedMessage, messageBytes, messageString);
              }
              break;
            case "answer":
              string answer = commandMessage.Parameters.Trim();
              if(!String.IsNullOrWhiteSpace(answer))
              {
                AnswerCommandReceived?.Invoke(null, new AnswerCommandEventArgs(receivedMessage, messageBytes, messageString, commandMessage, answer));
              }
              else
              {
                NotifyBadMessageReceived(receivedMessage, messageBytes, messageString);
              }
              break;

            default:
              // Send a generic event about the message being received
              //NotifyMessageReceived(receivedMessage, messageBytes, messageString);
              UnknownCommandReceived?.Invoke(null, new CommandEventArgs(receivedMessage, messageBytes, messageString, commandMessage));
              break;
          }
        } else
        {
          NotifyBadMessageReceived(receivedMessage, messageBytes, messageString);
        }

        // Go ahead and "complete" every message, even if we don't know we've processed it
        // We'll take the "UDP" approach for now. Consider doing this only after you know a message
        // has been successfully processed in the future...
        await deviceClient.CompleteAsync(receivedMessage);
      }
    }


    static void NotifyBadMessageReceived(Message Message, byte[] MessageBytes, string MessageString)
    {
      BadMessageReceived?.Invoke(null, new MessageEventArgs(Message, MessageBytes, MessageString));
    }

    static void NotifyMessageReceived(Message Message, byte[] MessageBytes, string MessageString)
    {
      MessageReceived?.Invoke(null, new MessageEventArgs(Message,MessageBytes,MessageString));
    }

  }
}
