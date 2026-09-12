using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public int GetCurrentStoryID()
    {
        if (GameDataManager.Instance == null)
            return 1;

        return GameDataManager.Instance.CurrentStoryID;
    }

    public bool IsStoryActive(int npcStoryID)
    {
        return npcStoryID == GetCurrentStoryID();
    }

    public bool IsStoryCompleted(int npcStoryID)
    {
        if (npcStoryID == 0)
            return false;

        return npcStoryID < GetCurrentStoryID();
    }

    public bool IsStoryLocked(int npcStoryID)
    {
        if (npcStoryID == 0)
            return false;

        return npcStoryID > GetCurrentStoryID();
    }

    public void CompleteStory(int completedStoryID)
    {
        if (GameDataManager.Instance == null)
            return;

        if (completedStoryID != GameDataManager.Instance.CurrentStoryID)
            return;

        GameDataManager.Instance.AdvanceStory();
    }
}