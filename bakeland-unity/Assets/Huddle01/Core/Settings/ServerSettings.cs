using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Huddle01.Core.Settings 
{
    public class ServerSettings
    {
        private string ProdServer;

        public string Port;

        private string WebSocketProdServerUrl;

        public string WebSocketUrl
        {
            get
            {
                return WebSocketProdServerUrl;
            }
            set
            {
                WebSocketProdServerUrl = value;
            }
        }
    }
}

