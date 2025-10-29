// Simple overlay HUD with: Objective (bold, below the top bar), Controls, and Grade popup.

using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    static HUD _instance;
    public static HUD Ensure()
    {
        if (_instance) return _instance;
        var go = new GameObject("HUD");
        _instance = go.AddComponent<HUD>();
        Object.DontDestroyOnLoad(go);
        _instance.Build();
        return _instance;
    }

    Canvas _canvas;
    Text _objective;
    Text _controls;
    Text _grade;

    void Build()
    {
        // Canvas
        _canvas = new GameObject("HUDCanvas").AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.gameObject.AddComponent<CanvasScaler>();
        _canvas.gameObject.AddComponent<GraphicRaycaster>();
        DontDestroyOnLoad(_canvas.gameObject);

        // Objective — BOLD and lowered so it doesn't block the awkwardness bar
        // Awk bar sits at ~y=-16 (top). We put this at ~y=-58.
        _objective = MakeText(
            name: "Objective",
            anchor: new Vector2(0.5f, 1f),
            pivot: new Vector2(0.5f, 1f),
            size: new Vector2(1000, 48),
            pos: new Vector2(0, -58),
            align: TextAnchor.UpperCenter,
            fontSize: 20,
            bold: true
        );
        _objective.color = Color.white;
        _objective.supportRichText = true;

        // Controls — small helper text at bottom center
        _controls = MakeText(
            "Controls",
            new Vector2(0.5f, 0f),
            new Vector2(0.5f, 0f),
            new Vector2(1100, 30),
            new Vector2(0, 10),
            TextAnchor.LowerCenter,
            16,
            bold: false
        );
        _controls.color = new Color(1f, 1f, 1f, 0.85f);
        _controls.enabled = false;

        // Grade — big centered message on win
        _grade = MakeText(
            "Grade",
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            new Vector2(1000, 90),
            Vector2.zero,
            TextAnchor.MiddleCenter,
            28,
            bold: true
        );
        _grade.color = new Color(1f, 0.92f, 0.25f, 1f);
        _grade.enabled = false;
    }

    Text MakeText(string name, Vector2 anchor, Vector2 pivot, Vector2 size, Vector2 pos, TextAnchor align, int fontSize, bool bold)
    {
        var go = new GameObject(name);
        go.transform.SetParent(_canvas.transform, false);

        var t = go.AddComponent<Text>();
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = fontSize;
        t.alignment = align;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Truncate;
        t.supportRichText = true;
        if (bold) t.fontStyle = FontStyle.Bold;

        var rt = t.rectTransform;
        rt.anchorMin = rt.anchorMax = anchor;
        rt.pivot = pivot;
        rt.sizeDelta = size;
        rt.anchoredPosition = pos;

        return t;
    }


    public void ShowObjective(string msg)
    {
        Ensure();
        if (_objective == null) return;

        // Keep it bold, wrap in <b> if caller didn't provide rich text.
        _objective.fontStyle = FontStyle.Bold;
        if (!string.IsNullOrEmpty(msg) && !msg.Contains("<b>"))
            msg = $"<b>{msg}</b>";

        _objective.text = msg;
        _objective.enabled = !string.IsNullOrEmpty(msg);
    }

    public void ShowControls()
    {
        Ensure();
        if (_controls == null) return;
        _controls.text = "WASD/Arrows: Move  •  Space: Dash  •  E: Hide/Unhide  •  Q: Fake Call  •  R: Restart";
        _controls.enabled = true;
    }

    public void ShowGrade(string msg)
    {
        Ensure();
        if (_grade == null) return;
        _grade.text = msg;
        _grade.enabled = true;
    }

    public void HideGrade()
    {
        Ensure();
        if (_grade == null) return;
        _grade.enabled = false;
        _grade.text = "";
    }
}
