using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiBrainBasic.Models
{
  public class MessageEventArgs : EventArgs
  {

    public MessageEventArgs(Message Message, byte[] MessageBytes, string MessageString)
    {
      this.Message = Message;
      this.MessageBytes = MessageBytes;
      this.MessageString = MessageString;
    }
    public Message Message { get; set; }

    public byte[] MessageBytes { get; set; }

    public string MessageString { get; set; }
  }
}
