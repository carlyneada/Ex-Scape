using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class FriendLookout : MonoBehaviour
{
    [Header("Indicator")]
    [Tooltip("Use '!' for guaranteed glyph.")]
    public string symbol = "!";
    public int fontSize = 60;
    public Color color = new Color(1f, 0.15f, 0.15f, 1f);

    [Header("Behavior")]
    [Tooltip("How often to check if an Ex is alerting/chasing.")]
    public float checkInterval = 0.10f;
    [Tooltip("If true, only show when the player is near this friend.")]
    public bool requireProximity = false;
    public float proximityRadius = 7.5f;

    [Header("Motion")]
    public float yOffset = 1.2f;     // world units above the head
    public float bobAmplitude = 0.15f;
    public float bobSpeed = 2.6f;

    Canvas _canvas;
    Text _text;
    float _baseY;
    float _t;
    bool _wasAlerting;

    void Start()
    {
        // World-space canvas for the indicator
        _canvas = new GameObject("FriendIndicatorCanvas").AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.WorldSpace;
        _canvas.overrideSorting = true;     // ensure it draws on top
        _canvas.sortingOrder = 200;

        var rt = _canvas.GetComponent<RectTransform>();
        rt.SetParent(transform, false);
        rt.localScale = Vector3.one * 0.01f; // crisp pixel look
        rt.sizeDelta = new Vector2(100, 80);

        _canvas.transform.localPosition = new Vector3(0, yOffset, -0.1f);
        _baseY = yOffset;

        // Text indicator
        _text = new GameObject("Indicator").AddComponent<Text>();
        _text.transform.SetParent(_canvas.transform, false);
        _text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        _text.fontSize = fontSize;
        _text.alignment = TextAnchor.MiddleCenter;
        _text.color = color;
        _text.text = symbol;

        var tr = _text.GetComponent<RectTransform>();
        tr.anchorMin = Vector2.zero; tr.anchorMax = Vector2.one;
        tr.offsetMin = Vector2.zero; tr.offsetMax = Vector2.zero;

        _text.enabled = false; // start hidden

        InvokeRepeating(nameof(Pulse), 0.05f, checkInterval);
    }

    void Update()
    {
        if (_text != null && _text.enabled)
        {
            // Gentle bobbing only while visible
            _t += Time.deltaTime * bobSpeed * Mathf.PI * 2f;
            float bob = Mathf.Sin(_t) * bobAmplitude;
            _canvas.transform.localPosition = new Vector3(0, _baseY + bob, -0.1f);
        }
    }

    void Pulse()
    {
        bool alertNow = IsAnyExAlerting();
        if (requireProximity) alertNow &= IsPlayerNear();

        if (alertNow && !_wasAlerting)
        {
            _text.enabled = true;
            AudioKit.PlayPing(transform.position, 0.7f); // one-time ping on entering alert
        }
        else if (!alertNow && _wasAlerting)
        {
            _text.enabled = false;
        }

        _wasAlerting = alertNow;
    }

    bool IsAnyExAlerting()
    {
        var player = FindObjectOfType<PlayerController>();
        if (!player) return false;

        var exes = FindObjectsOfType<EnemyController>();
        for (int i = 0; i < exes.Length; i++)
        {
            var ex = exes[i];
            if (ex == null) continue;

            // Primary: rely on EnemyController.IsChasing
            if (ex.IsChasing) return true;

            // Fallback: if we don't have chase yet, consider "seeing the player" as alert
            if (ex.fov != null && ex.fov.CanSee(player)) return true;
        }
        return false;
    }

    bool IsPlayerNear()
    {
        var player = FindObjectOfType<PlayerController>();
        if (!player) return false;
        return Vector2.Distance(player.transform.position, transform.position) <= proximityRadius;
    }
}
