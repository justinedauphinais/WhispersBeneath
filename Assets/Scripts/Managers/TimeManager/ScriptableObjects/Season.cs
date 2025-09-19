using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Season", menuName = "Time System/Season")]
public class Season : ScriptableObject
{
    public string Name;
    public List<Events> events;
}
