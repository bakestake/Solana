using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Huddle01.Core.EventBroadcast 
{
    public class EventsBroadcaster
    {
        public delegate void RaiseEventHandler(Response.ResponseOneofCase ResCase, Response reponse);

        public event RaiseEventHandler OnEventRaised;
        
        public void RaiseEvent(Response.ResponseOneofCase ResCase, Response reponse) 
        {
            OnEventRaised?.Invoke(ResCase,reponse);
        }
    }
}


