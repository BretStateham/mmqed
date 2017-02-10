using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiBrainBasic.Models
{
  public class DeviceToCloudMessage
  {
    public DateTime SentAt { get; set; } = DateTime.Now;
    public string Team { get; set; }
    public string MessageText { get; set; }
  }
}
