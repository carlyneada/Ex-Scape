using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event Action OnWin;
    public event Action OnFail;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Win()  => OnWin?.Invoke();
    public void Fail() => OnFail?.Invoke();

    public void RestartScene()
    {
        var s = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        UnityEngine.SceneManagement.SceneManager.LoadScene(s.name);
    }

    // Back-compat
    public void Restart() => RestartScene();
}
