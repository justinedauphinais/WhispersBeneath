using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDialogue", menuName = "Dialogue System/CharacterDialogue")]
public class CharacterDialogue : ScriptableObject
{
    [SerializeField] private DialogueAsset meetingDialogue;
    [SerializeField] private DialogueAsset level2event;
    [SerializeField] private DialogueAsset[] level1Idle;
    [SerializeField] private DialogueAsset[] level2Idle;
    [SerializeField] private DialogueAsset[] level3Idle;
    [SerializeField] private DialogueAsset[] level4Idle;
    [SerializeField] private DialogueAsset[] level5Idle;
    [SerializeField] private DialogueAsset[] level6Idle;
    [SerializeField] private DialogueAsset[] level7Idle;
    [SerializeField] private DialogueAsset[] level8Idle;
    [SerializeField] private DialogueAsset[] level9Idle;
    [SerializeField] private DialogueAsset[] level10Idle;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public DialogueAsset GetMeetingDialogue()
    {
        return meetingDialogue;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public DialogueAsset GetEvent(int level)
    {
        switch (level)
        {
            case 2:
                return level2event;

        }

        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public DialogueAsset GetRandomPerLevel(int level)
    {
        switch (level)
        {
            case 1:
                return level1Idle[UnityEngine.Random.Range(0, level1Idle.Length)];

            case 2:
                return level2Idle[UnityEngine.Random.Range(0, level2Idle.Length)];

            case 3:
                return level3Idle[UnityEngine.Random.Range(0, level3Idle.Length)];

            case 4:
                return level4Idle[UnityEngine.Random.Range(0, level4Idle.Length)];

            case 5:
                return level5Idle[UnityEngine.Random.Range(0, level5Idle.Length)];

            case 6:
                return level6Idle[UnityEngine.Random.Range(0, level6Idle.Length)];

            case 7:
                return level7Idle[UnityEngine.Random.Range(0, level7Idle.Length)];

            case 8:
                return level8Idle[UnityEngine.Random.Range(0, level8Idle.Length)];

            case 9:
                return level9Idle[UnityEngine.Random.Range(0, level9Idle.Length)];

            case 10:
                return level10Idle[UnityEngine.Random.Range(0, level10Idle.Length)];

        }

        return null;
    }
}
