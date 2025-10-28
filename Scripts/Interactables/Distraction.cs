// Assets/Scripts/Interactables/Distraction.cs
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Distraction : MonoBehaviour, IInteractable
{
    public string Prompt => "Trigger Distraction (E)";

    void Awake()
    {
        var col = GetComponent<Collider2D>(); col.isTrigger = true;
    }

    public void Interact(PlayerController player)
    {
        Noise.Emit(transform.position);
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
