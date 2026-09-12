using System;
using UnityEngine;

public enum DialogueSpeaker
{
    NPC,
    Player
}

[Serializable]
public class DialogueLine
{
    [Header("Dialogue")]
    public DialogueSpeaker speaker;

    [TextArea(2, 5)]
    public string text;

    [Header("Character Portrait")]
    [Tooltip("Drag sprite karakter untuk baris dialog ini.")]
    public Sprite portrait;

    [Header("Dialogue Sound")]
    [Tooltip("Drag AudioClip yang dimainkan ketika dialog ini muncul.")]
    public AudioClip sound;
}