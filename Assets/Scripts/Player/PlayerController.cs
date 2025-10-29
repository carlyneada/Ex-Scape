using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 4.5f;
    public float dashSpeed = 8.5f;
    public float dashTime = 0.15f;
    public float dashCooldown = 0.35f;

    [Header("Stamina")]
    public float staminaMax = 1f;
    public float staminaRegenPerSec = 0.55f;
    public float staminaDashCost = 0.5f;

    [Header("Fake Call")]
    public float fakeCallDuration = 1.6f; // lowers detection briefly
    public float fakeCallCooldown = 4.0f;

    Rigidbody2D _rb;
    SpriteRenderer _sr;

    float _stamina;
    bool _isDashing;
    float _dashTimer;
    float _dashCooldownTimer;

    bool _hidden;
    float _fakeCallTimer;
    float _fakeCallCooldownTimer;

    Vector2 _move;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _stamina = staminaMax;
        SetHidden(false);
    }

    void Update()
    {
        // Restart (handled by GameFlow without scene reload)
        if (Input.GetKeyDown(KeyCode.R))
        {
            GameFlow.Ensure().QuickRestart();
            return;
        }

        // Movement input
        _move = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        // Fake call (Q) — only if off cooldown
        _fakeCallCooldownTimer -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Q) && _fakeCallCooldownTimer <= 0f)
        {
            TriggerFakeCall();
        }

        // Dash (Space)
        _dashCooldownTimer -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space) && !_isDashing && _dashCooldownTimer <= 0f && _stamina >= staminaDashCost * 0.95f)
        {
            _isDashing = true;
            _dashTimer = dashTime;
            _dashCooldownTimer = dashCooldown;
            _stamina = Mathf.Max(0f, _stamina - staminaDashCost);
        }

        // Stamina regen
        if (!_isDashing) _stamina = Mathf.Min(staminaMax, _stamina + staminaRegenPerSec * Time.deltaTime);

        // Fake call timer tick
        if (_fakeCallTimer > 0f) _fakeCallTimer -= Time.deltaTime;

        // Visual feedback for hidden
        _sr.color = _hidden ? new Color(1f,1f,1f,0.35f) : Color.white;
    }

    void FixedUpdate()
    {
        Vector2 v = Vector2.zero;

        if (_isDashing)
        {
            _dashTimer -= Time.fixedDeltaTime;
            v = _move * dashSpeed;

            if (_dashTimer <= 0f)
                _isDashing = false;
        }
        else
        {
            v = _move * moveSpeed;
        }

        _rb.velocity = v;

        // Face direction (no spin)
        if (v.sqrMagnitude > 0.0001f)
        {
            transform.localScale = new Vector3(Mathf.Sign(v.x) == 0 ? 1f : Mathf.Sign(v.x), 1f, 1f);
        }
    }

    // ----- Public API used by other systems -----

    public void SetHidden(bool value)
    {
        bool changed = (_hidden != value);
        _hidden = value;

        // Play a little "bloop" only when we successfully enter hidden state.
        if (changed && _hidden)
        {
            AudioKit.PlayHide(transform.position);
        }
    }

    public bool IsHidden() => _hidden;

    public bool IsFakeCalling() => _fakeCallTimer > 0f;

    public float GetStamina01() => Mathf.Clamp01(_stamina / Mathf.Max(0.0001f, staminaMax));

    // Simple multiplier that DetectionMeter (or FOV) can read if you wired it that way.
    public float CurrentDetectionRateMultiplier()
    {
        // When fake calling, reduce how quickly awkwardness rises.
        return IsFakeCalling() ? 0.4f : 1f;
    }

    // ----- Internals -----

    void TriggerFakeCall()
    {
        _fakeCallTimer = fakeCallDuration;
        _fakeCallCooldownTimer = fakeCallCooldown;

        // Play a quick "ring" trill so the player knows it fired.
        AudioKit.PlayFakeCall(transform.position);

        // (Optional) quick tiny visual nudge (slight scale punch)
        StopAllCoroutines();
        StartCoroutine(PunchScale());
    }

    System.Collections.IEnumerator PunchScale()
    {
        float t = 0f;
        Vector3 baseS = Vector3.one;
        while (t < 0.10f)
        {
            t += Time.deltaTime;
            float k = 1f + Mathf.Sin(t * Mathf.PI / 0.10f) * 0.08f;
            transform.localScale = new Vector3(baseS.x * k, baseS.y * k, baseS.z);
            yield return null;
        }
        transform.localScale = baseS;
    }
}
