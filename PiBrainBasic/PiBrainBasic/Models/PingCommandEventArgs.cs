using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiBrainBasic.Models
{
  public class PingCommandEventArgs : CommandEventArgs
  {
    public string Payload { get; set; }

    public PingCommandEventArgs(Message Message, byte[] MessageBytes, string MessageString, CloudToDeviceMessage CommandMessage, string Payload)
      : base(Message, MessageBytes, MessageString, CommandMessage)
    {
      this.Payload = Payload;
    }
  }
}
