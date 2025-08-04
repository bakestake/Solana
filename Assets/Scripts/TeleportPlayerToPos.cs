using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPlayerToPos : MonoBehaviour
{
    public Transform teleport;
    public bool useFader;

    public void Teleport(Transform teleport, bool fader)
    {
        StartCoroutine(TeleportRoutine(teleport, fader));
    }

    IEnumerator TeleportRoutine(Transform teleport, bool fader)
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            if (fader)
            {
                LoadingWheel.instance.EnableFader();
                yield return new WaitForSeconds(0.5f);
                player.transform.position = teleport.position;
                LoadingWheel.instance.DisableFader();
            }
            else
            {
                player.transform.position = teleport.position;
            }
        }
    }

    public void Teleport()
    {
        StartCoroutine(TeleportRoutine2(teleport, useFader));
    }

    IEnumerator TeleportRoutine2(Transform teleport, bool fader)
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            if (fader)
            {
                LoadingWheel.instance.EnableFader();
                yield return new WaitForSeconds(0.5f);
                player.transform.position = teleport.position;
                LoadingWheel.instance.DisableFader();
            }
            else
            {
                player.transform.position = teleport.position;
            }
        }
    }
}
