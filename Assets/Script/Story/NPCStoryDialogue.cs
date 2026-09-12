using System.Collections.Generic;
using UnityEngine;

public class NPCStoryDialogue : MonoBehaviour
{
    [Header("Story")]
    [Tooltip("0 = NPC biasa, tidak berpengaruh pada story.")]
    [SerializeField] private int storyID = 0;

    [Header("Story Dialogue")]
    [SerializeField] private List<DialogueLine> dialogueSequence = new();

    [Header("Locked Dialogue")]
    [Tooltip("Muncul jika Story ID NPC belum aktif.")]
    [SerializeField] private List<DialogueLine> lockedDialogue = new();

    [Header("Completed Dialogue")]
    [Tooltip("Muncul jika story NPC sudah selesai.")]
    [SerializeField] private List<DialogueLine> completedDialogue = new();

    [Header("Normal NPC Dialogue")]
    [Tooltip("Digunakan hanya jika Story ID = 0.")]
    [SerializeField] private List<DialogueLine> normalDialogue = new();

    public int StoryID => storyID;

    public string NPCName => gameObject.name;

    public List<DialogueLine> DialogueSequence => dialogueSequence;

    public List<DialogueLine> LockedDialogue => lockedDialogue;

    public List<DialogueLine> CompletedDialogue => completedDialogue;

    public List<DialogueLine> NormalDialogue => normalDialogue;
}