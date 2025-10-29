using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Collider2D))]
public class Distraction : MonoBehaviour, IInteractable
{
    // Text shown by the interaction system (e.g., “E: Trigger Distraction”)
    public string Prompt => "Trigger Distraction (E)";

    void Awake()
    {
        // Make sure our collider is a trigger so the player can stand “in” it.
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    // Called by the interaction system when the player presses E while in range.
    public void Interact(PlayerController player)
    {
        // Emit a noise signal that enemies can react to (implementation lives in Noise).
        Noise.Emit(transform.position);

        // Quick visual feedback: fade the sprite slightly, then restore.
        var sr = GetComponent<SpriteRenderer>();
        if (sr) sr.color = new Color(1f, 1f, 1f, 0.6f);
        Invoke(nameof(ResetTint), 0.2f);
    }

    void ResetTint()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr) sr.color = Color.white;
    }
}