// Assets/Scripts/World/StandPad.cs
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider2D))]
public class StandPad : MonoBehaviour
{
    public float holdTime = 1.0f;
    public string label = "SELECT";
    public Color labelColor = Color.white;

    System.Action _callback;
    float _t;
    bool _inside;
    Text _text;

    public void Init(string labelText, float timeToTrigger, System.Action onSelect)
    {
        label = labelText; holdTime = timeToTrigger; _callback = onSelect;
        var col = GetComponent<BoxCollider2D>(); col.isTrigger = true;
        MakeLabel();
    }

    void MakeLabel()
    {
        var canvasGO = new GameObject("PadLabel");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 20;

        var rect = canvas.GetComponent<RectTransform>();
        rect.SetParent(transform, false);
        rect.localScale = Vector3.one * 0.01f;
        rect.sizeDelta = new Vector2(220, 60);
        rect.localPosition = new Vector3(0, 0.9f, 0);

        var txtGO = new GameObject("Text");
        txtGO.transform.SetParent(canvas.transform, false);
        _text = txtGO.AddComponent<Text>();
        _text.alignment = TextAnchor.MiddleCenter;
        _text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        _text.text = label + "  (stand)";
        _text.color = labelColor;
        _text.raycastTarget = false;

        var tr = _text.GetComponent<RectTransform>();
        tr.anchorMin = Vector2.zero; tr.anchorMax = Vector2.one; tr.offsetMin = Vector2.zero; tr.offsetMax = Vector2.zero;
    }

    void OnTriggerEnter2D(Collider2D c) { if (c.GetComponent<PlayerController>()) { _inside = true; _t = 0f; } }
    void OnTriggerExit2D(Collider2D c)  { if (c.GetComponent<PlayerController>()) { _inside = false; _t = 0f; } }

    void Update()
    {
        if (!_inside || _callback == null) return;
        _t += Time.deltaTime;
        if (_text) _text.text = label + $"  ({Mathf.CeilToInt(Mathf.Max(holdTime - _t, 0f))}s)";
        if (_t >= holdTime) { var cb = _callback; _callback = null; cb(); }
    }
}
