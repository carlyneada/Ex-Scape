using UnityEngine;

public class Boot
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Go()
    {
        if (GameManager.Instance == null)
        {
            var gm = new GameObject("GameManager");
            gm.AddComponent<GameManager>();
        }

        // Ensure global hotkeys are always listening (R to restart)
        if (!Object.FindObjectOfType<Hotkeys>())
        {
            new GameObject("Hotkeys").AddComponent<Hotkeys>();
        }

        GameFlow.Ensure().BuildSplash();
    }
}
