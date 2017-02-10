using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiBrainBasic.Models
{
  public class CommandEventArgs : MessageEventArgs
  {

    public CloudToDeviceMessage CommandMessage { get; set; }

    public CommandEventArgs(Message Message, byte[] MessageBytes, string MessageString, CloudToDeviceMessage CommandMessage) 
      : base(Message,MessageBytes,MessageString)
    {
      this.CommandMessage = CommandMessage;
    }
  }
}
