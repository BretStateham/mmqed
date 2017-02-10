using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiBrainBasic.Models
{
  public class AnswerCommandEventArgs : CommandEventArgs
  {
    public string Answer { get; set; }

    public AnswerCommandEventArgs(Message Message, byte[] MessageBytes, string MessageString, CloudToDeviceMessage CommandMessage, string Answer)
      : base(Message, MessageBytes, MessageString, CommandMessage)
    {
      this.Answer = Answer;
    }
  }
}
