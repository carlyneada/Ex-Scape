// Assets/Scripts/Framework/Hotkeys.cs
using UnityEngine;

[DefaultExecutionOrder(-300)]
public class Hotkeys : MonoBehaviour
{
    void Awake() { DontDestroyOnLoad(gameObject); }

    void Update()
    {
        // Quick in-place restart: rebuild the current level instead of reloading the scene.
        if (Input.GetKeyDown(KeyCode.R))
        {
            GameFlow.Ensure().QuickRestart();
        }
    }
}
