using UnityEngine;
using UnityEngine.InputSystem;

public class NPCInteraction : MonoBehaviour
{
    [SerializeField] private NPCStoryDialogue npcDialogue;

    private bool playerInside;

    private void Awake()
    {
        if (npcDialogue == null)
        {
            npcDialogue = GetComponent<NPCStoryDialogue>();
        }
    }

    private void Update()
    {
        if (!playerInside)
            return;

        if (DialogueManager.Instance == null)
            return;

        if (DialogueManager.Instance.IsDialogueActive())
            return;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Interact();
        }
    }

    private void Interact()
    {
        if (npcDialogue == null)
            return;

        int npcStoryID = npcDialogue.StoryID;

        // =====================================================
        // STORY ID = 0
        // NPC BIASA
        // =====================================================

        if (npcStoryID == 0)
        {
            DialogueManager.Instance.StartNormalDialogue(
                npcDialogue
            );

            return;
        }

        // =====================================================
        // STORY BELUM TERBUKA
        // =====================================================

        if (npcStoryID > StoryManager.Instance.GetCurrentStoryID())
        {
            DialogueManager.Instance.StartLockedDialogue(
                npcDialogue
            );

            return;
        }

        // =====================================================
        // STORY SUDAH SELESAI
        // =====================================================

        if (npcStoryID < StoryManager.Instance.GetCurrentStoryID())
        {
            DialogueManager.Instance.StartCompletedDialogue(
                npcDialogue
            );

            return;
        }

        // =====================================================
        // STORY SEDANG AKTIF
        // =====================================================

        if (npcStoryID == StoryManager.Instance.GetCurrentStoryID())
        {
            DialogueManager.Instance.StartStoryDialogue(
                npcDialogue
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}