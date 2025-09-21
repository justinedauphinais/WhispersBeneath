using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JournalUIController : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject entriesParent;
    [SerializeField] private GameObject prefabEntry;

    [Header("Selected entry")]
    [SerializeField] private GameObject selectedPanel;
    [SerializeField] private TMP_Text entryName;
    [SerializeField] private TMP_Text entryDescription;
    [SerializeField] private Image entryCharacter;

    /// <summary>
    /// 
    /// </summary>
    public void Awake()
    {
        HideSelf();
    }

    /// <summary>
    /// 
    /// </summary>
    public void HideSelf()
    {
        panel.SetActive(false);
        selectedPanel.SetActive(false);

        Quest_UI[] quests = entriesParent.GetComponentsInChildren<Quest_UI>();

        foreach (Quest_UI quest in quests)
        {
            Destroy(quest.gameObject);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void DisplaySelf()
    {
        panel.SetActive(true);
        GameObject.FindWithTag("StateManager").GetComponent<StateManager>().SetState(StateManager.GameState.Pause);
        QuestHolder questHolder = GameObject.FindWithTag("Player").GetComponent<QuestHolder>();

        if (questHolder.Quests.Count == 0)
        {
            // Show empty
        }

        foreach (Quest quest in questHolder.Quests)
        {
            Quest_UI q = Instantiate(prefabEntry, entriesParent.transform).GetComponent<Quest_UI>();
            q.Initialize(quest);
            q.GetComponent<Button>().onClick.AddListener(() => OnClickQuest(q));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="quest"></param>
    public void OnClickQuest(Quest_UI quest)
    {
        ViewQuest(quest.AssociatedQuest);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="quest"></param>
    public void ViewQuest(Quest quest)
    {
        selectedPanel.SetActive(true);
        entryName.text = quest.Name;
        entryDescription.text = quest.Description;
        entryCharacter.sprite = quest.AssociatedNPC.characterImage;
    }
}
