using UnityEngine;

[CreateAssetMenu(fileName = "QuestGraveCleaning", menuName = "Quests/Grave cleaning")]
public class QuestGraveCleaning : Quest
{
    [SerializeField] private int amountNeeded;
    private int currentAmount;

    public override void Initialize(QuestHolder questHolder, NPCInteractive npc)
    {
        questHolder.OnGraveDigging += IncreaseAmount;
        associatedNPC = npc;
        currentAmount = 0;
        type = QuestType.GraveCleaning;
    }

    public void IncreaseAmount()
    {
        currentAmount += 1;

        if (currentAmount >= amountNeeded)
            CompleteQuest();
    }
}
