using UnityEngine;

public class NPCRelationship : MonoBehaviour
{
    public float Relationship = 0f;

    public int Level = 1;

    public bool EventReady = false;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="amount"></param>
    public void AddToRelationship(float amount)
    {
        Relationship = Mathf.Clamp((Relationship + amount), 0f, 100f);

        if ((Mathf.FloorToInt(Relationship / 10) + 1) > (Mathf.FloorToInt(((Relationship - amount) / 10)) + 1)) {
            EventReady = true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public int GetRelationshipLevel(out bool TriggerEvent)
    {
        int level = Mathf.FloorToInt((Relationship / 10)) + 1;
        TriggerEvent = EventReady;
        EventReady = false;
        return Mathf.FloorToInt((Relationship / 10)) + 1;
    }
}
