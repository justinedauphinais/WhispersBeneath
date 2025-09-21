using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Quests/Quest")]
public class Quest : ScriptableObject
{
    [SerializeField] protected string questName;
    [SerializeField] protected string description;
    [SerializeField] protected int relationshipModifier;

    [SerializeField] protected MoneyAmount reward;

    protected QuestType type;

    protected NPCInteractive associatedNPC;

    public string Name => name;
    public string Description => description;
    public NPCInteractive AssociatedNPC => associatedNPC;
    public MoneyAmount Reward => reward;
    public QuestType Type => type;

    public virtual void Initialize(QuestHolder questHolder, NPCInteractive npc)
    {
        associatedNPC = npc;
    }

    public virtual bool CompleteQuest()
    {
        Debug.Log($"Quest {this.name} finished");
        GameObject.FindWithTag("Player").GetComponentInChildren<PlayerMoneyManagement>().AddMoney(reward);
        GameObject.FindWithTag("Player").GetComponentInChildren<QuestHolder>().QuestCompleted(this);
        associatedNPC.Relationsip.AddToRelationship(relationshipModifier);

        return true;
    }
}

public enum QuestType
{
    Normal,
    GraveCleaning
}