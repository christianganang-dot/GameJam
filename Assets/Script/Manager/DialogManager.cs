using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialogueBox;

    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Character Portrait")]
    [SerializeField] private Image playerPortrait;
    [SerializeField] private Image npcPortrait;

    [Header("Portrait Colors")]
    [SerializeField] private Color activePortraitColor = Color.white;

    [SerializeField]
    private Color inactivePortraitColor =
        new Color(0.55f, 0.55f, 0.55f, 1f);

    [Header("Audio")]
    [SerializeField] private AudioSource dialogueAudioSource;

    [Header("Player")]
    [SerializeField] private string playerName = "Player";

    [Header("Typing")]
    [SerializeField] private float typingSpeed = 0.03f;

    private NPCStoryDialogue currentNPC;
    private List<DialogueLine> currentDialogue;

    private int currentLineIndex;

    private Coroutine typingCoroutine;

    private bool isTyping;
    private bool isDialogueActive;

    private DialogueMode currentMode;

    private enum DialogueMode
    {
        Normal,
        Locked,
        Story,
        Completed
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (dialogueBox != null)
        {
            dialogueBox.SetActive(false);
        }

        if (playerPortrait != null)
        {
            playerPortrait.gameObject.SetActive(false);
        }

        if (npcPortrait != null)
        {
            npcPortrait.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isDialogueActive)
            return;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            NextDialogue();
        }
    }

    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }

    // =========================================================
    // STORY
    // =========================================================

    public void StartStoryDialogue(NPCStoryDialogue npc)
    {
        StartDialogue(
            npc,
            npc.DialogueSequence,
            DialogueMode.Story
        );
    }

    // =========================================================
    // NORMAL NPC
    // =========================================================

    public void StartNormalDialogue(NPCStoryDialogue npc)
    {
        StartDialogue(
            npc,
            npc.NormalDialogue,
            DialogueMode.Normal
        );
    }

    // =========================================================
    // LOCKED
    // =========================================================

    public void StartLockedDialogue(NPCStoryDialogue npc)
    {
        StartDialogue(
            npc,
            npc.LockedDialogue,
            DialogueMode.Locked
        );
    }

    // =========================================================
    // COMPLETED
    // =========================================================

    public void StartCompletedDialogue(NPCStoryDialogue npc)
    {
        StartDialogue(
            npc,
            npc.CompletedDialogue,
            DialogueMode.Completed
        );
    }

    // =========================================================
    // START DIALOGUE
    // =========================================================

    private void StartDialogue(
        NPCStoryDialogue npc,
        List<DialogueLine> dialogue,
        DialogueMode mode)
    {
        if (npc == null)
            return;

        if (dialogue == null || dialogue.Count == 0)
        {
            Debug.LogWarning(
                $"NPC '{npc.NPCName}' tidak memiliki dialogue."
            );

            return;
        }

        currentNPC = npc;
        currentDialogue = dialogue;
        currentMode = mode;

        currentLineIndex = 0;

        isDialogueActive = true;

        if (dialogueBox != null)
        {
            dialogueBox.SetActive(true);
        }

        // Portrait dimulai dari kosong
        if (playerPortrait != null)
        {
            playerPortrait.gameObject.SetActive(false);
        }

        if (npcPortrait != null)
        {
            npcPortrait.gameObject.SetActive(false);
        }

        ShowCurrentLine();
    }

    // =========================================================
    // SHOW CURRENT LINE
    // =========================================================

    private void ShowCurrentLine()
    {
        if (currentDialogue == null)
        {
            EndDialogue();
            return;
        }

        if (currentLineIndex >= currentDialogue.Count)
        {
            EndCurrentDialogue();
            return;
        }

        DialogueLine line =
            currentDialogue[currentLineIndex];

        // =========================
        // SPEAKER NAME
        // =========================

        if (line.speaker == DialogueSpeaker.Player)
        {
            nameText.text = playerName;
        }
        else
        {
            nameText.text = currentNPC.NPCName;
        }

        // =========================
        // PORTRAIT
        // =========================

        UpdatePortrait(line);

        // =========================
        // SOUND
        // =========================

        PlayDialogueSound(line);

        // =========================
        // TYPEWRITER
        // =========================

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(
            TypeText(line.text)
        );
    }

    // =========================================================
    // PORTRAIT
    // =========================================================

    private void UpdatePortrait(DialogueLine line)
    {
        // =========================================
        // PLAYER SEDANG BERBICARA
        // =========================================

        if (line.speaker == DialogueSpeaker.Player)
        {
            // Update portrait Player jika Element punya sprite baru
            if (playerPortrait != null)
            {
                if (line.portrait != null)
                {
                    playerPortrait.sprite = line.portrait;
                    playerPortrait.gameObject.SetActive(true);
                }

                // Player terang
                playerPortrait.color = activePortraitColor;
            }

            // NPC menjadi gelap
            if (npcPortrait != null &&
                npcPortrait.gameObject.activeSelf)
            {
                npcPortrait.color = inactivePortraitColor;
            }
        }

        // =========================================
        // NPC SEDANG BERBICARA
        // =========================================

        else
        {
            // Update portrait NPC jika Element punya sprite baru
            if (npcPortrait != null)
            {
                if (line.portrait != null)
                {
                    npcPortrait.sprite = line.portrait;
                    npcPortrait.gameObject.SetActive(true);
                }

                // NPC terang
                npcPortrait.color = activePortraitColor;
            }

            // Player menjadi gelap
            if (playerPortrait != null &&
                playerPortrait.gameObject.activeSelf)
            {
                playerPortrait.color = inactivePortraitColor;
            }
        }
    }

    // =========================================================
    // SOUND
    // =========================================================

    private void PlayDialogueSound(DialogueLine line)
    {
        if (dialogueAudioSource == null)
            return;

        // Hentikan sound dari dialog sebelumnya
        dialogueAudioSource.Stop();

        if (line.sound == null)
            return;

        dialogueAudioSource.clip = line.sound;
        dialogueAudioSource.Play();
    }

    // =========================================================
    // TYPEWRITER
    // =========================================================

    private IEnumerator TypeText(string text)
    {
        isTyping = true;

        dialogueText.text = "";

        foreach (char letter in text)
        {
            dialogueText.text += letter;

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        typingCoroutine = null;
    }

    // =========================================================
    // SPACE
    // =========================================================

    private void NextDialogue()
    {
        // Kalau masih mengetik:
        // SPACE hanya menyelesaikan text.
        if (isTyping)
        {
            CompleteTyping();
            return;
        }

        // Kalau sudah selesai mengetik:
        // pindah ke Element berikutnya.
        currentLineIndex++;

        ShowCurrentLine();
    }

    // =========================================================
    // SKIP TYPEWRITER
    // =========================================================

    private void CompleteTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (currentDialogue == null)
            return;

        if (currentLineIndex >= currentDialogue.Count)
            return;

        dialogueText.text =
            currentDialogue[currentLineIndex].text;

        isTyping = false;
    }

    // =========================================================
    // END CURRENT DIALOGUE
    // =========================================================

    private void EndCurrentDialogue()
    {
        // Hanya Story Dialogue yang menaikkan progress story
        if (currentMode == DialogueMode.Story)
        {
            if (StoryManager.Instance != null &&
                currentNPC != null)
            {
                StoryManager.Instance.CompleteStory(
                    currentNPC.StoryID
                );
            }
        }

        EndDialogue();
    }

    // =========================================================
    // END DIALOGUE
    // =========================================================

    private void EndDialogue()
    {
        if (playerPortrait != null)
        {
            playerPortrait.sprite = null;
            playerPortrait.color = activePortraitColor;
            playerPortrait.gameObject.SetActive(false);
        }

        if (npcPortrait != null)
        {
            npcPortrait.sprite = null;
            npcPortrait.color = activePortraitColor;
            npcPortrait.gameObject.SetActive(false);
        }

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (dialogueAudioSource != null)
        {
            dialogueAudioSource.Stop();
        }

        isTyping = false;
        isDialogueActive = false;

        currentNPC = null;
        currentDialogue = null;

        currentLineIndex = 0;

        if (playerPortrait != null)
        {
            playerPortrait.sprite = null;
            playerPortrait.gameObject.SetActive(false);
        }

        if (npcPortrait != null)
        {
            npcPortrait.sprite = null;
            npcPortrait.gameObject.SetActive(false);
        }

        if (dialogueBox != null)
        {
            dialogueBox.SetActive(false);
        }

        dialogueText.text = "";
        nameText.text = "";
    }

    // =========================================================
    // PLAYER NAME
    // =========================================================

    public string GetPlayerName()
    {
        return playerName;
    }

    public void SetPlayerName(string newName)
    {
        if (!string.IsNullOrWhiteSpace(newName))
        {
            playerName = newName;
        }
    }
}