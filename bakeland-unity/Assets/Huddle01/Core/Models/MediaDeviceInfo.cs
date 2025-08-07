using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Huddle01.Core.Models 
{
    public class MediaDeviceInfo
    {
        public string DeviceId { get; set; }
        public string GroupId { get; set; }
        public int HashCode { get; set; }
        public string Lable { get; set; }

        public string Kind { get; set; }
    }
}


