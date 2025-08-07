using System.Collections;
using System.Collections.Generic;
using Org.BouncyCastle.Asn1.Misc;
using UnityEngine;

[System.Serializable]
public class QuestData
{
    public string id;
    public QuestState state;
    public int questStepIndex;
    public QuestStepState[] questStepStates;

    public QuestData(string id, QuestState state, int questStepIndex, QuestStepState[] questStepStates)
    {
        this.id = id;
        this.state = state;
        this.questStepIndex = questStepIndex;
        this.questStepStates = questStepStates;
    }
}
