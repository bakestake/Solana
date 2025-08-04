using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using Photon.Voice.Unity;


public class PushToTalk : MonoBehaviour
{
    //private Recorder recorder;
    private bool IsTalking = false;

    //private void Awake()
    //{
    //    if (recorder == null)
    //    {
    //        recorder = GetComponent<Recorder>();
    //    }
    //}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && !IsTalking)
        {
            IsTalking = true;
            WantToTalk();
        }
        else if(Input.GetKeyUp(KeyCode.T) && IsTalking)
        {
            IsTalking = false;
            NotTalking();
        }
    }
    private void WantToTalk()
    {
        //recorder.TransmitEnabled = true;
    }

    private void NotTalking()
    {
        //recorder.TransmitEnabled = false;
    }
}
