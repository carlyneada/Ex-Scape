using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    // Global instance (read-only from the outside).
    public static GameManager Instance { get; private set; }

    // Simple signals the rest of the game can subscribe to.
    public event Action OnWin;
    public event Action OnFail;

    void Awake()
    {
        // Standard singleton guard: if another instance exists, kill this one.
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }

        Instance = this;
        DontDestroyOnLoad(gameObject); // persist across scene loads
    }

    // Fire the Win event (listeners decide what to do: show grade, advance level, etc.)
    public void Win()  => OnWin?.Invoke();

    // Fire the Fail event (listeners handle building the "caught" screen).
    public void Fail() => OnFail?.Invoke();

    // Utility: reloads the currently active scene by name.
    // (We usually prefer code-built restarts in GameFlow, but this is handy to have.)
    public void RestartScene()
    {
        var s = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        UnityEngine.SceneManagement.SceneManager.LoadScene(s.name);
    }

    // Back-compat alias so older calls still work.
    public void Restart() => RestartScene();
}