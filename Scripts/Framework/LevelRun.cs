// Assets/Scripts/Framework/LevelRun.cs
// Tracks elapsed time for the current run and reports the current awkwardness (0..1)
// from DetectionMeter. Used by GameFlow to show grade on win.

using UnityEngine;

public class LevelRun : MonoBehaviour
{
    [System.Serializable]
    public struct RunSnapshot
    {
        public float time;        // seconds since level start
        public float awkward01;   // 0..1 current awkwardness
    }

    float _elapsed;
    DetectionMeter _meter;

    void Awake()
    {
        // Find the player's DetectionMeter once at start.
        var player = FindObjectOfType<PlayerController>();
        if (player) _meter = player.GetComponent<DetectionMeter>();
        _elapsed = 0f;
    }

    void Update()
    {
        _elapsed += Time.deltaTime;
    }

    // Called by GameFlow to display results.
    public RunSnapshot Snapshot()
    {
        float awkward = _meter ? _meter.current01 : 0f;
        return new RunSnapshot { time = _elapsed, awkward01 = awkward };
    }
}
