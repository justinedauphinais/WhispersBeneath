using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "DialogueAsset", menuName = "Dialogue System/DialogueAsset")]
[Serializable]
public class DialogueAsset : ScriptableObject
{
    public Dialogue[] dialogues;
    public bool ShowOnce;
    public bool Seen;
}

[Serializable]
public class Dialogue
{
    public string dialogue;
    public int relationshipAffectation;
    public int nextDialogue;
    public Dialogue[] options;
}
