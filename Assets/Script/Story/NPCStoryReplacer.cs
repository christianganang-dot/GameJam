using System.Collections;
using UnityEngine;

public class NPCStoryReplacer : MonoBehaviour
{
    [Header("Story Condition")]
    [Tooltip("Saat Current Story ID mencapai angka ini, NPC akan diganti.")]
    [SerializeField] private int replaceAtStoryID = 2;

    [Header("Replacement")]
    [Tooltip("Prefab NPC yang akan menggantikan NPC ini. Kosongkan jika hanya ingin NPC lama hilang.")]
    [SerializeField] private GameObject replacementPrefab;

    [Tooltip("Opsional. Jika kosong, NPC baru muncul di posisi NPC lama.")]
    [SerializeField] private Transform replacementSpawnPoint;

    private bool hasBeenReplaced;

    private void Start()
    {
        if (GameDataManager.Instance == null)
        {
            Debug.LogWarning(
                $"{gameObject.name}: GameDataManager tidak ditemukan."
            );

            return;
        }

        GameDataManager.Instance.OnStoryIDChanged += OnStoryIDChanged;

        // Penting untuk kasus ketika scene baru dimuat.
        CheckStory(GameDataManager.Instance.CurrentStoryID);
    }

    private void OnDestroy()
    {
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.OnStoryIDChanged -= OnStoryIDChanged;
        }
    }

    private void OnStoryIDChanged(int currentStoryID)
    {
        CheckStory(currentStoryID);
    }

    private void CheckStory(int currentStoryID)
    {
        if (hasBeenReplaced)
            return;

        if (currentStoryID < replaceAtStoryID)
            return;

        hasBeenReplaced = true;

        // Kita tunggu 1 frame supaya aman kalau perubahan
        // Story ID terjadi tepat ketika Dialogue selesai.
        StartCoroutine(ReplaceNextFrame());
    }

    private IEnumerator ReplaceNextFrame()
    {
        yield return null;

        ReplaceNPC();
    }

    private void ReplaceNPC()
    {
        if (replacementPrefab != null)
        {
            Vector3 spawnPosition =
                replacementSpawnPoint != null
                    ? replacementSpawnPoint.position
                    : transform.position;

            Quaternion spawnRotation =
                replacementSpawnPoint != null
                    ? replacementSpawnPoint.rotation
                    : transform.rotation;

            Instantiate(
                replacementPrefab,
                spawnPosition,
                spawnRotation,
                transform.parent
            );
        }

        // NPC lama hilang.
        Destroy(gameObject);
    }
}