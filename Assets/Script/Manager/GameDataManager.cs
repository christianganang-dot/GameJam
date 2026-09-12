using System;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    [Header("Player")]
    [SerializeField] private string playerName = "Player";

    [Header("Story")]
    [SerializeField] private int currentStoryID = 1;

    public string PlayerName => playerName;
    public int CurrentStoryID => currentStoryID;

    // Sistem lain bisa mendengarkan perubahan Story ID.
    public event Action<int> OnStoryIDChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Tetap hidup ketika pindah scene.
        DontDestroyOnLoad(gameObject);
    }

    // =========================================================
    // PLAYER
    // =========================================================

    public void SetPlayerName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            return;

        playerName = newName;
    }

    // =========================================================
    // STORY
    // =========================================================

    public void SetCurrentStoryID(int newStoryID)
    {
        // Story 0 kita gunakan untuk NPC normal,
        // jadi Current Story minimal 1.
        newStoryID = Mathf.Max(1, newStoryID);

        if (currentStoryID == newStoryID)
            return;

        currentStoryID = newStoryID;

        OnStoryIDChanged?.Invoke(currentStoryID);
    }

    public void AdvanceStory()
    {
        SetCurrentStoryID(currentStoryID + 1);
    }
}