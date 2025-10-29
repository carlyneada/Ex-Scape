using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider2D))]
public class StandPad : MonoBehaviour
{
    public float holdTime = 1.0f;       // seconds the player must stand on pad
    public string label = "SELECT";     // label text shown above the pad
    public Color labelColor = Color.white;

    System.Action _callback;            // action to invoke when held long enough
    float _t;                           // current stand timer
    bool _inside;                       // is the player currently on the pad?
    Text _text;                         // reference to the label text

    // Call this after spawning to set text/hold-time and hook the callback.
    public void Init(string labelText, float timeToTrigger, System.Action onSelect)
    {
        label = labelText; 
        holdTime = timeToTrigger; 
        _callback = onSelect;

        // Ensure our collider is a trigger so the player can stand inside.
        var col = GetComponent<BoxCollider2D>(); 
        col.isTrigger = true;

        MakeLabel();
    }

    // Builds a small world-space canvas + text above the pad.
    void MakeLabel()
    {
        var canvasGO = new GameObject("PadLabel");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 20; // render above most sprites

        var rect = canvas.GetComponent<RectTransform>();
        rect.SetParent(transform, false);
        rect.localScale = Vector3.one * 0.01f;      // scale UI to world size
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
        tr.anchorMin = Vector2.zero; 
        tr.anchorMax = Vector2.one; 
        tr.offsetMin = Vector2.zero; 
        tr.offsetMax = Vector2.zero;
    }

    // Player stepped onto the pad: start counting.
    void OnTriggerEnter2D(Collider2D c) 
    { 
        if (c.GetComponent<PlayerController>()) 
        { 
            _inside = true; 
            _t = 0f; 
        } 
    }

    // Player stepped off the pad: stop and reset timer.
    void OnTriggerExit2D(Collider2D c)  
    { 
        if (c.GetComponent<PlayerController>()) 
        { 
            _inside = false; 
            _t = 0f; 
            if (_text) _text.text = label + "  (stand)";
        } 
    }

    // While the player is inside, advance the timer and update the countdown.
    void Update()
    {
        if (!_inside || _callback == null) return;

        _t += Time.deltaTime;

        // Show a simple second countdown (ceil so it hits 0 at trigger).
        if (_text) 
            _text.text = label + $"  ({Mathf.CeilToInt(Mathf.Max(holdTime - _t, 0f))}s)";

        // Fire once, then null the callback to prevent double-invoke.
        if (_t >= holdTime) 
        { 
            var cb = _callback; 
            _callback = null; 
            cb(); 
        }
    }
}