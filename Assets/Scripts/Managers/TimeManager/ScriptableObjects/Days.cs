using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Days", menuName = "Time System/Days")]
public class Days : ScriptableObject
{
    public int index;
    public List<Events> events;
}
