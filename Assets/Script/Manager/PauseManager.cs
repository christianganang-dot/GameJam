using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    public static bool IsPaused { get; private set; }

    [Header("Pause UI")]
    [SerializeField] private GameObject pauseMenu;

    [Header("Audio")]
    [SerializeField] private bool pauseAudio = true;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Pastikan game normal saat mulai
        IsPaused = false;
        Time.timeScale = 1f;

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (IsPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (IsPaused)
            return;

        IsPaused = true;

        // Freeze waktu game
        Time.timeScale = 0f;

        // Pause semua audio gameplay
        if (pauseAudio)
        {
            AudioListener.pause = true;
        }

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        if (!IsPaused)
            return;

        IsPaused = false;

        // Jalankan waktu kembali
        Time.timeScale = 1f;

        if (pauseAudio)
        {
            AudioListener.pause = false;
        }

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        // Safety supaya Time.timeScale tidak tertinggal 0
        // jika object dihancurkan.
        if (Instance == this)
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;

            IsPaused = false;
            Instance = null;
        }
    }
}