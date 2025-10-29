using UnityEngine;

/// Tracks "awkwardness" (0..1). Increases when visible to Exes, decreases otherwise.
/// When it reaches 1, triggers fail.
public class DetectionMeter : MonoBehaviour
{
    public float increasePerSec = 0.65f; // base rate when fully visible to one Ex
    public float decayPerSec    = 0.35f; // cool-down rate when not seen
    public float current01 { get; private set; } // 0..1

    PlayerController _player;

    void Awake()
    {
        _player = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (!_player) return;

        // Sum visibility from all enemies (clamped 0..1.5 for multiple exes adding pressure).
        float visSum = 0f;
        var exes = FindObjectsOfType<EnemyController>();
        for (int i = 0; i < exes.Length; i++)
        {
            if (exes[i] == null) continue;
            visSum += exes[i].VisibilityOfPlayer();
        }
        visSum = Mathf.Clamp(visSum, 0f, 1.5f);

        // Apply player's fake-call multiplier (slows the increase rate).
        float rateMul = _player.CurrentDetectionRateMultiplier(); // 0.4f during fake call, else 1
        if (visSum > 0f)
        {
            current01 += visSum * increasePerSec * rateMul * Time.deltaTime;
        }
        else
        {
            current01 -= decayPerSec * Time.deltaTime;
        }

        current01 = Mathf.Clamp01(current01);

        if (current01 >= 1f)
        {
            // Fail once; GameManager handles showing Caught screen.
            if (GameManager.Instance) GameManager.Instance.Fail();
            enabled = false;
        }
    }
}
