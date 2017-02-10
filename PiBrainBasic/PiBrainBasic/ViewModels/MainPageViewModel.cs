using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using PiBrainBasic.Services;
using Windows.Storage;
using Microsoft.Azure.Devices.Client;
using Template10.Services.SerializationService;
using System.Text;
using PiBrainBasic.Models;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace PiBrainBasic.ViewModels
{
  public class MainPageViewModel : ViewModelBase
  {

    DelegateCommand<String> playSoundCommand;
    public DelegateCommand<String> PlaySoundCommand
       => playSoundCommand ?? (playSoundCommand = new DelegateCommand<String>(async (path) => await PlaySoundAsync(path), (path) => true));
    private async Task PlaySoundAsync(String Path)
    {
      await SoundService.PlayAudioFileAsync(Path);
    }

    private ObservableCollection<String> messagesReceived;

    public ObservableCollection<String> MessagesReceived
    {
      get { return messagesReceived; }
      set {
        messagesReceived = value;
        Set(ref messagesReceived, value);
      }
    }

    private ObservableCollection<String> messagesSent;

    public ObservableCollection<String> MessagesSent
    {
      get { return messagesSent; }
      set
      {
        messagesSent= value;
        Set(ref messagesSent, value);
      }
    }

    
    


    public MainPageViewModel()
    {
      MessagesReceived = new ObservableCollection<String>();
      MessagesSent = new ObservableCollection<String>();

      IoTHubDeviceService.PingCommandReceived += IoTHubDeviceService_PingCommandReceived;
      IoTHubDeviceService.BrewCommandReceived += IoTHubDeviceService_BrewCommandReceived;
      IoTHubDeviceService.MessageReceived += IoTHubDeviceService_MessageReceived;
      IoTHubDeviceService.BadMessageReceived += IoTHubDeviceService_BadMessageReceived;
      IoTHubDeviceService.TakePictureCommandReceived += IoTHubDeviceService_TakePictureCommandReceived;
      IoTHubDeviceService.AnswerCommandReceived += IoTHubDeviceService_AnswerCommandReceived;
      IoTHubDeviceService.UnknownCommandReceived += IoTHubDeviceService_UnknownCommandReceived;

    }

    private async void IoTHubDeviceService_BrewCommandReceived(object sender, BrewCommandEventArgs e)
    {
      await SoundService.PlayAudioFileAsync("brew.wav");
      string msg = "Brewing Coffee!";
      SendDeviceMessage(e.CommandMessage.Team, e.Message.MessageId, msg);
    }

    private async void IoTHubDeviceService_PingCommandReceived(object sender, PingCommandEventArgs e)
    {
      await SoundService.PlayAudioFileAsync("ping.wav");
      string msg = String.Format("Ping Response: {0}",e.Payload);
      SendDeviceMessage(e.CommandMessage.Team, e.Message.MessageId, msg);
    }

    private async void SendDeviceMessage(string Team, string correlationId, string MessageText)
    {
      DeviceToCloudMessage deviceMessage = new DeviceToCloudMessage() { Team = Team, MessageText = MessageText};
      Debug.WriteLine(MessageText);
      MessagesSent.Insert(0, MessageText);
      await IoTHubDeviceService.SendDeviceToCloudMessagesAsync(deviceMessage, correlationId);
    }

    private async void IoTHubDeviceService_UnknownCommandReceived(object sender, CommandEventArgs e)
    {
      await SoundService.PlayAudioFileAsync("laserwoowoo.wav");
      string msg = String.Format("Unknown command received from Team {0}: {1}", e.CommandMessage.Team, e.CommandMessage.Command);
      SendDeviceMessage(e.CommandMessage.Team,e.Message.MessageId, msg);
    }

    private async void IoTHubDeviceService_AnswerCommandReceived(object sender, AnswerCommandEventArgs e)
    {
      string answer = e.Answer.Trim().ToLower();
      if(answer == "ksm248")
      {
        await SoundService.PlayAudioFileAsync("fanfare.wav");
        string msg = String.Format("Correct answer received by team {0}: {1}", e.CommandMessage.Team, answer);
        SendDeviceMessage(e.CommandMessage.Team, e.Message.MessageId, msg);
      }
      else
      {
        //Wrong answer
        await SoundService.PlayAudioFileAsync("wronganswer.wav");
        string msg = String.Format("Wrong answer received by team {0}: {1}", e.CommandMessage.Team, answer);
        SendDeviceMessage(e.CommandMessage.Team, e.Message.MessageId, msg);

      }
    }

    private async void IoTHubDeviceService_TakePictureCommandReceived(object sender, TakePictureCommandEventArgs e)
    {
      //Using a zero based index... Hmm...
      if (e.Camera >= 0 && e.Camera < CameraService.Cameras.Count)
      {
        await SoundService.PlayAudioFileAsync("pictake01.wav");
        //Debug.WriteLine(String.Format("Taking picture for Team {0} with camera: {1}", e.CommandMessage.Team, e.Camera));
        try
        {
          //string fileName = await CameraService.ProcessTakePictureCommand(e);
          string fileName = await CameraService.ProcessTakePictureCommandFixed(e);
          string msg = String.Format("Team {0}, the photo from camera {1} has been uploaded to '{0}-<correlation-id>.jpg'", e.CommandMessage.Team, e.Camera);
          SendDeviceMessage(e.CommandMessage.Team, e.Message.MessageId, msg);
        }
        catch(Exception ex)
        {
          string msg = String.Format("Team {0}, there was a problem uploading the photo from camera {1}.", e.CommandMessage.Team, e.Camera);
          SendDeviceMessage(e.CommandMessage.Team, e.Message.MessageId, msg);
        }
      }
      else
      {
        await SoundService.PlayAudioFileAsync("soundeffect02.wav");
        string msg = String.Format("Team {0}, the following camera is invalid: {1}", e.CommandMessage.Team, e.Camera);
        SendDeviceMessage(e.CommandMessage.Team, e.Message.MessageId, msg);
      }
    }

    private async void IoTHubDeviceService_BadMessageReceived(object sender, MessageEventArgs e)
    {
      await SoundService.PlayAudioFileAsync("allyourbase.wav");
      Debug.WriteLine(String.Format("Received Bad message: {0}", e.MessageString));
    }

    private async void IoTHubDeviceService_MessageReceived(object sender, MessageEventArgs e)
    {
      //await SoundService.PlayAudioFileAsync("alienvoice01.wav");
      MessagesReceived.Insert(0, e.MessageString);
      Debug.WriteLine(String.Format("Received message: {0}", e.MessageString));
    }

    public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
    {
      await IoTHubDeviceService.StartListening();
      await CameraService.FindCameras();
      await Task.CompletedTask;
    }
  }

}

