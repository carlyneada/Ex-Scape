// Assets/Scripts/Enemies/FieldOfView2D.cs
using UnityEngine;

public class FieldOfView2D : MonoBehaviour
{
    [Header("Vision")]
    public float viewRadius = 7.5f;          // was ~6, give them a longer reach
    [Range(10f, 180f)]
    public float viewAngle = 110f;           // was ~70, wider cone feels more responsive
    [Range(0.2f, 2f)]
    public float visibilityFactor = 1.0f;

    [Header("Snappier Recognition")]
    [Tooltip("Within this distance, Ex recognizes player instantly (ignores angle).")]
    public float instantChaseDistance = 2.25f;
    [Tooltip("If true, FOV faces along current velocity (fallback to sprite flip).")]
    public bool useVelocityAsForward = true;

    /// Returns true if this FOV can currently see the player.
    public bool CanSee(PlayerController player)
    {
        if (!player) return false;
        if (player.IsHidden()) return false;

        Vector2 toPlayer = player.transform.position - transform.position;
        float dist = toPlayer.magnitude;

        // Instant recognition up close (ignores angle)
        if (dist <= instantChaseDistance) return true;

        // Fake call reduces effective radius a bit
        float radius = viewRadius * (player.IsFakeCalling() ? 0.75f : 1f);
        if (dist > radius) return false;

        // Determine forward direction for the cone
        Vector2 forward = GetForwardVector();

        // Angle gate
        float angle = Vector2.Angle(forward, toPlayer.normalized);
        if (angle > viewAngle * 0.5f) return false;

        // (Optional) add a Physics2D.Linecast here for walls if you add colliders
        return true;
    }

    /// 0..1 visibility strength (used by DetectionMeter to aggregate).
    public float VisibilityStrength(PlayerController player)
    {
        if (!CanSee(player)) return 0f;
        float d = Vector2.Distance(player.transform.position, transform.position);
        float r = Mathf.Max(0.001f, viewRadius);
        float near = Mathf.Clamp01(1f - (d / r)); // 1 close, 0 far
        return Mathf.Clamp01(near * visibilityFactor);
    }

    Vector2 GetForwardVector()
    {
        if (useVelocityAsForward)
        {
            var rb = GetComponent<Rigidbody2D>();
            if (rb != null && rb.velocity.sqrMagnitude > 0.02f)
                return rb.velocity.normalized;
        }
        // Fallback: sprite flip determines facing (left/right)
        float sx = Mathf.Sign(transform.localScale.x);
        if (sx == 0f) sx = 1f;
        return new Vector2(sx, 0f);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.25f);
        Vector3 pos = transform.position;
        Vector2 forward = Application.isPlaying ? GetForwardVector() : Vector2.right;
        float half = viewAngle * 0.5f;

        Vector2 a = Quaternion.Euler(0, 0, +half) * forward * viewRadius;
        Vector2 b = Quaternion.Euler(0, 0, -half) * forward * viewRadius;

        Gizmos.DrawLine(pos, pos + (Vector3)a);
        Gizmos.DrawLine(pos, pos + (Vector3)b);
        Gizmos.DrawWireSphere(pos, instantChaseDistance);
    }
#endif
}
