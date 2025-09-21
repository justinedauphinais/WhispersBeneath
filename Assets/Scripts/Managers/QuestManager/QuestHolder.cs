using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestHolder : MonoBehaviour
{
    [SerializeField] private List<Quest> quests = new List<Quest>();

    public Action OnGraveDigging;

    public List<Quest> Quests => quests;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="quest"></param>
    public void AddQuest(Quest quest, NPCInteractive npc)
    {
        quest.Initialize(this, npc);
        quests.Add(quest);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="quest"></param>
    public bool QuestCompleted(Quest quest)
    {
        return quests.Remove(quest);
    }
}
