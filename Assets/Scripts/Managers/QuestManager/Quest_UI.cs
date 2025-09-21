using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Quest_UI : MonoBehaviour
{
    [Header("Frontend")]
    [SerializeField] private TMP_Text questName;
    [SerializeField] private Image questCharacterImage;

    [Header("Backend")]
    private Quest associatedQuest;

    public Quest AssociatedQuest => associatedQuest;

    public void Initialize(Quest quest)
    {
        this.questName.text = quest.Name;
        this.questCharacterImage.sprite = quest.AssociatedNPC.characterImage;
        this.associatedQuest = quest;
    }
}
