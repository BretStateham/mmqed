using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiBrainBasic.Models
{
  public class TakePictureCommandEventArgs : CommandEventArgs
  {
    public int Camera { get; set; }

    public TakePictureCommandEventArgs(Message Message, byte[] MessageBytes, string MessageString, CloudToDeviceMessage CommandMessage, int Camera) 
      : base(Message,MessageBytes,MessageString,CommandMessage)
    {
      this.Camera = Camera;
    }
  }
}
