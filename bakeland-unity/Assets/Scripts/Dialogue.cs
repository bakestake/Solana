using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue System/Dialogue")]
public class Dialogue : ScriptableObject
{
    public bool isRandom;
    public Message[] messages;
    public Actor[] actors;
}
[System.Serializable]
public class Message
{
    public int actorId;
    [TextArea(3, 7)]
    public string message;
}
[System.Serializable]
public class Actor
{
    public string Name;
    public Sprite sprite;
    public Color color = new Color(91f / 255f, 59f / 255f, 19f / 255f, 1);

}
