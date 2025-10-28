// Assets/Scripts/Framework/GameFlow.cs
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameFlow : MonoBehaviour
{
    public static GameFlow Ensure()
    {
        var gf = FindObjectOfType<GameFlow>();
        if (gf) return gf;
        var go = new GameObject("GameFlow");
        gf = go.AddComponent<GameFlow>();
        Object.DontDestroyOnLoad(gf.gameObject);
        return gf;
    }

    enum LevelId { None, Cafe, Campus, Street, Market, Metro, Party }
    LevelId currentLevel = LevelId.None;

    Transform _root;
    LevelRun _run;
    PlayerController _player;

    void Awake()
    {
        if (GameManager.Instance == null)
        {
            var gm = new GameObject("GameManager");
            gm.AddComponent<GameManager>();
        }
        GameManager.Instance.OnWin  += OnWin;
        GameManager.Instance.OnFail += OnFail;
    }

    // Global quick restart (Hotkey: R from PlayerController)
    public void QuickRestart()
    {
        switch (currentLevel)
        {
            case LevelId.Cafe:   BuildLevel_Cafe();   break;
            case LevelId.Campus: BuildLevel_Campus(); break;
            case LevelId.Street: BuildLevel_Street(); break;
            case LevelId.Market: BuildLevel_Market(); break;
            case LevelId.Metro:  BuildLevel_Metro();  break;
            case LevelId.Party:  BuildLevel_Party();  break;
            default:             BuildLevelSelect();  break;
        }
    }

    // ================= Splash / Select / How-To ===================

    public void BuildSplash()
    {
        Clear(); PurgeStaminaUI();
        var cam = EnsureCamera();
        HUD.Ensure().HideGrade();
        HUD.Ensure().ShowObjective("");

        New("SplashBG", PixelKit.MakeTileSprite('b', 22, 14), new Vector3(0,0,5))
           .GetComponent<SpriteRenderer>().sortingOrder = -5;

        MakeWorldText("Ex-Scape", new Vector3(0, 2.6f, 0), 36);
        MakeWorldText("Stand on the GREEN pad to START", new Vector3(0, 1.4f, 0), 18);
        MakeWorldText("Legend: GREEN pads = select/continue  •  YELLOW tile = EXIT", new Vector3(0, -2.6f, 0), 16);

        _player = SpawnPlayer(new Vector3(-6, -3, 0));
        MakePad(new Vector3(-1.8f, -0.2f, 0), 2, 1, 'g', "START", BuildLevelSelect);
        MakePad(new Vector3( 1.8f, -0.2f, 0), 2, 1, 'g', "HOW TO", ShowHowTo);

        cam.orthographicSize = 6f;
        ScreenBarriers();
    }

    public void BuildLevelSelect()
    {
        Clear(); PurgeStaminaUI();
        HUD.Ensure().HideGrade();
        HUD.Ensure().ShowObjective("Level Select — stand on a GREEN pad");
        HUD.Ensure().ShowControls();

        EnsureCamera();
        Backdrop_StreetHub();

        _player = SpawnPlayer(new Vector3(-7, -3.2f, 0));

        // Top row (L1–L3)
        MakePad(new Vector3(-4f,  0.8f, 0), 2, 1, 'g', "CAFE (L1)",   BuildLevel_Cafe);
        MakePad(new Vector3( 0f,  0.8f, 0), 2, 1, 'g', "CAMPUS (L2)", BuildLevel_Campus);
        MakePad(new Vector3( 4f,  0.8f, 0), 2, 1, 'g', "STREET (L3)", BuildLevel_Street);

        // Bottom row (L4–L6)
        MakePad(new Vector3(-4f, -2.0f, 0), 2, 1, 'g', "MARKET (L4)", BuildLevel_Market);
        MakePad(new Vector3( 0f, -2.0f, 0), 2, 1, 'g', "METRO (L5)",  BuildLevel_Metro);
        MakePad(new Vector3( 4f, -2.0f, 0), 2, 1, 'g', "PARTY (L6)",  BuildLevel_Party);

        ScreenBarriers();
    }

    public void ShowHowTo()
    {
        Clear(); PurgeStaminaUI();
        HUD.Ensure().HideGrade();
        HUD.Ensure().ShowObjective("How to Play");
        HUD.Ensure().ShowControls();

        EnsureCamera();
        New("Floor", PixelKit.MakeTileSprite('b', 22, 14), new Vector3(0,0,5))
            .GetComponent<SpriteRenderer>().sortingOrder = -5;

        _player = SpawnPlayer(new Vector3(-6, -2.5f, 0));

        var info =
            "Reach the YELLOW EXIT without filling the Awkwardness bar.\n" +
            "WASD/Arrows move  •  Space dash  •  E hide/trigger  •  Q fake call\n" +
            "Legend: GREEN pads = select/continue • YELLOW tile = EXIT";

        MakeWorldText(info, new Vector3(0, 2.4f, 0), 20);
        MakePad(new Vector3(0, -0.6f, 0), 3, 1, 'g', "BACK", BuildSplash);

        ScreenBarriers();
    }

    // ========================= Levels =============================

    public void BuildLevel_Cafe()
    {
        currentLevel = LevelId.Cafe;
        BuildLevelCommon("Goal: Reach the YELLOW EXIT. Avoid the Ex. E = hide / distract.",
            Backdrop_Cafe,
            () =>
            {
                BoundsWalls();

                Cover(new Vector3(-2,  0, 0), 2, 1);
                Cover(new Vector3( 2, -1, 0), 2, 1);
                Cover(new Vector3(-5,  2, 0), 2, 1);
                Distraction(new Vector3(3, 2, 0));

                Ex(new Vector3(-6, 1.5f, 0), new Vector3(-6,1.5f,0), new Vector3(6,1.5f,0));

                Exit(RandomCorner(new Vector3(9, 6, 0), new Vector3(-9, 6, 0)));
                Friend(new Vector3(0, -3, 0));
            });
    }

    public void BuildLevel_Campus()
    {
        currentLevel = LevelId.Campus;
        BuildLevelCommon("Goal: Cross the quad to the YELLOW EXIT. Q slows detection.",
            Backdrop_Campus,
            () =>
            {
                BoundsWalls();

                Cover(new Vector3(-4, -1, 0), 3, 1);
                Cover(new Vector3( 1,  1, 0), 3, 1);
                Cover(new Vector3(-1,  3, 0), 2, 1);

                Distraction(new Vector3(4, -2, 0));

                Ex(new Vector3(-6,  0.5f, 0), new Vector3(-6,0.5f,0), new Vector3(6,0.5f,0));
                Ex(new Vector3( 6, -2.5f, 0), new Vector3(6,-2.5f,0), new Vector3(-2,-2.5f,0));

                Exit(RandomCorner(new Vector3(9, 6, 0), new Vector3(9, -6, 0)));
                Friend(new Vector3(-1, -1.5f, 0));
            });
    }

    public void BuildLevel_Street()
    {
        currentLevel = LevelId.Street;
        BuildLevelCommon("Goal: Slip past and reach the YELLOW EXIT in the alley.",
            Backdrop_Street,
            () =>
            {
                BoundsWalls();

                Cover(new Vector3(-3,  0, 0), 2, 1);
                Cover(new Vector3( 0, -1, 0), 2, 1);
                Cover(new Vector3( 4,  1, 0), 2, 1);
                Distraction(new Vector3(5, -2, 0));

                ExLane(-8.5f, 8.5f,  3.8f, true);
                ExLane( 8.5f,-6.5f,  0.6f, true);
                ExLane(-7.5f, 6.5f, -2.6f, true);

                Exit(RandomCorner(new Vector3(9, -6, 0), new Vector3(9, 6, 0)));
                Friend(new Vector3(0, -2, 0));
            });
    }

    public void BuildLevel_Market()
    {
        currentLevel = LevelId.Market;
        BuildLevelCommon("MARKET: Tight aisles. More patrols. Plan your dashes.",
            Backdrop_Market,
            () =>
            {
                BoundsWalls();

                Cover(new Vector3(-5,  3, 0), 2, 1);
                Cover(new Vector3(-1,  1, 0), 2, 1);
                Cover(new Vector3( 3, -1, 0), 2, 1);
                Cover(new Vector3( 6,  2, 0), 2, 1);

                Distraction(new Vector3(-2, -3, 0));
                Distraction(new Vector3( 5,  4, 0));

                ExLane(-8.5f, 8.5f,  4.5f, true);
                ExLane( 8.5f,-8.5f,  2.5f, true);
                ExLane(-8.5f, 8.5f,  0.5f, true);
                ExLane( 8.5f,-8.5f, -2.5f, true);

                var spawnPos = new Vector3(-8, -6, 0);
                var exitPos = ChooseFarCorner(
                    spawnPos, 5.0f,
                    new Vector3(-9, -6, 0),
                    new Vector3( 9, -6, 0),
                    new Vector3(-9,  6, 0),
                    new Vector3( 9,  6, 0)
                );
                Exit(exitPos);

                Friend(new Vector3(-6, -3.5f, 0));
                Friend(new Vector3( 4,  3.5f, 0));
            });
    }

    public void BuildLevel_Metro()
    {
        currentLevel = LevelId.Metro;
        BuildLevelCommon("METRO: Platforms & trains. Very crowded patrols.",
            Backdrop_Metro,
            () =>
            {
                BoundsWalls();

                Cover(new Vector3(-6, -1.2f, 0), 3, 1);
                Cover(new Vector3( 0,  0.8f, 0), 3, 1);
                Cover(new Vector3( 6, -2.2f, 0), 3, 1);
                Distraction(new Vector3(0, -4.8f, 0));

                ExLane(-9f,  9f,  4.6f, true);
                ExLane( 9f, -9f,  2.8f, true);
                ExLane(-9f,  9f,  1.0f, true);
                ExLane( 9f, -9f, -0.8f, true);
                ExLane(-9f,  9f, -3.0f, true);

                Exit(RandomCorner(new Vector3(9, -6, 0), new Vector3(-9, 6, 0)));
                Friend(new Vector3(-2, -3.5f, 0));
                Friend(new Vector3( 7,  2.5f, 0));
            });
    }

    public void BuildLevel_Party()
    {
        currentLevel = LevelId.Party;
        BuildLevelCommon("PARTY: Packed space. 6 patrols. Use friends & fake calls.",
            Backdrop_Party,
            () =>
            {
                BoundsWalls();

                Cover(new Vector3(-6,  2.5f, 0), 2, 1);
                Cover(new Vector3(-2, -0.5f, 0), 2, 1);
                Cover(new Vector3( 2,  1.5f, 0), 2, 1);
                Cover(new Vector3( 6, -1.0f, 0), 2, 1);
                Distraction(new Vector3(0, -2.8f, 0));
                Distraction(new Vector3(7,  3.2f, 0));

                ExLane(-9f,  9f,  5.0f, true);
                ExLane( 9f, -9f,  3.2f, true);
                ExLane(-9f,  9f,  1.4f, true);
                ExLane( 9f, -9f, -0.4f, true);
                ExLane(-9f,  9f, -2.2f, true);
                ExLane( 9f, -9f, -4.0f, true);

                var spawnPos = new Vector3(-8, -6, 0);
                var exitPos = ChooseFarCorner(
                    spawnPos, 5.0f,
                    new Vector3( 9, -6, 0),
                    new Vector3(-9, -6, 0),
                    new Vector3( 9,  6, 0),
                    new Vector3(-9,  6, 0)
                );
                Exit(exitPos);

                Friend(new Vector3(-5, -3.5f, 0));
                Friend(new Vector3( 4,  3.5f, 0));
                Friend(new Vector3( 0,  0.0f, 0));
            });
    }

    // ================== Results / fail / end screens ==============

    void OnWin()
    {
        FreezeGameplay();

        if (_run != null)
        {
            var snap = _run.Snapshot();
            string grade = Grade(snap.time, snap.awkward01);
            HUD.Ensure().ShowGrade($"Level Clear!  Time: {snap.time:0.0}s  Awkward: {(int)(snap.awkward01*100)}%  Grade: {grade}");
        }

        StartCoroutine(AutoContinue(1.75f));
    }

    void OnFail()
    {
        FreezeGameplay();
        BuildCaught();
    }

    IEnumerator AutoContinue(float delay) { yield return new WaitForSeconds(delay); BuildNextLevel(); }

    void BuildNextLevel()
    {
        switch (currentLevel)
        {
            case LevelId.Cafe:   BuildLevel_Campus(); break;
            case LevelId.Campus: BuildLevel_Street(); break;
            case LevelId.Street: BuildLevel_Market(); break;
            case LevelId.Market: BuildLevel_Metro();  break;
            case LevelId.Metro:  BuildLevel_Party();  break;
            case LevelId.Party:  BuildAllClear();     break;
            default:             BuildLevelSelect();  break;
        }
    }

    void BuildAllClear()
    {
        currentLevel = LevelId.None;
        Clear(); PurgeStaminaUI(); NukeStrays();
        EnsureCamera();

        New("AllClearBG", PixelKit.MakeTileSprite('b', 22, 14), new Vector3(0,0,5))
            .GetComponent<SpriteRenderer>().sortingOrder = -5;

        MakeWorldText("You successfully avoided all your exes!", new Vector3(0, 1.6f, 0), 30);
        MakeWorldText("Stand on a pad to continue.", new Vector3(0, 0.8f, 0), 20);

        var menuPlayer = SpawnMenuPlayer(new Vector3(0, -2.4f, 0));

        MakePad(new Vector3(-4.2f, -4.0f, 0), 2, 1, 'g', "PLAY AGAIN (L1)", BuildLevel_Cafe);
        MakePad(new Vector3( 0.0f, -4.0f, 0), 2, 1, 'g', "LEVEL SELECT",    BuildLevelSelect);
        MakePad(new Vector3( 4.2f, -4.0f, 0), 2, 1, 'g', "TITLE",           BuildSplash);

        ScreenBarriers();
    }

    // ============== Common build path & helpers ===================

    public void BuildLevelCommon(string objective, System.Action backdrop, System.Action layout)
    {
        Clear(); PurgeStaminaUI(); NukeStrays();
        HUD.Ensure().HideGrade();
        HUD.Ensure().ShowControls();
        HUD.Ensure().ShowObjective(objective);

        EnsureAwkUI();
        EnsureCamera();
        backdrop?.Invoke();

        ScreenBarriers();

        _player = SpawnPlayer(new Vector3(-8, -6, 0));
        layout();

        if (_run) Destroy(_run.gameObject);
        _run = new GameObject("LevelRun").AddComponent<LevelRun>();

        StartCoroutine(FadeObjectiveSoon(3f));
    }

    void ScreenBarriers()
    {
        var cam = EnsureCamera();
        float hh = cam.orthographicSize;
        float hw = hh * cam.aspect;
        float inset = 0.25f;

        MakeBarrier(new Vector2(0,  -hh + inset), (hw * 2f) + 2f, 0.5f);
        MakeBarrier(new Vector2(0,   hh - inset), (hw * 2f) + 2f, 0.5f);
        MakeBarrier(new Vector2(-hw + inset, 0),  0.5f, (hh * 2f) + 2f);
        MakeBarrier(new Vector2( hw - inset, 0),  0.5f, (hh * 2f) + 2f);
    }

    void MakeBarrier(Vector2 center, float w, float h)
    {
        var go = New("Barrier", null, new Vector3(center.x, center.y, 0));
        var col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = false;
        col.size = new Vector2(w, h);
    }

    void BoundsWalls()
    {
        Block(new Vector2(0, -7.5f), 22, 1, 'w'); Block(new Vector2(0, 7.5f), 22, 1, 'w');
        Block(new Vector2(-11, 0), 1, 15, 'w');  Block(new Vector2(11, 0), 1, 15, 'w');
    }

    void ExLane(float startX, float endX, float y, bool kinematic)
    {
        Vector3 a = new Vector3(startX, y, 0);
        Vector3 b = new Vector3(endX,  y, 0);
        Ex(a, a, b);
    }

    Vector3 RandomCorner(Vector3 a, Vector3 b) => (Random.value < 0.5f) ? a : b;

    Vector3 ChooseFarCorner(Vector3 avoid, float minDist, params Vector3[] options)
    {
        Vector3 best = options[0];
        float bestDist = -1f;
        var good = new System.Collections.Generic.List<Vector3>();
        for (int i = 0; i < options.Length; i++)
        {
            float d = Vector2.Distance(avoid, options[i]);
            if (d >= minDist) good.Add(options[i]);
            if (d > bestDist) { bestDist = d; best = options[i]; }
        }
        if (good.Count > 0) return good[Random.Range(0, good.Count)];
        return best;
    }

    void EnsureAwkUI()
    {
        if (!FindObjectOfType<DetectionMeterUI>())
            new GameObject("AwkUI").AddComponent<DetectionMeterUI>();
    }

    void PurgeStaminaUI()
    {
        var sliders = FindObjectsOfType<Slider>(true);
        for (int i = 0; i < sliders.Length; i++)
            if (sliders[i].name == "StaminaBar") Destroy(sliders[i].gameObject);
        var maybe = GameObject.Find("StaminaUI");
        if (maybe) Destroy(maybe);
    }

    IEnumerator FadeObjectiveSoon(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        HUD.Ensure().ShowObjective("");
    }

    string Grade(float time, float awkward01)
    {
        int score = 0;
        if (time < 25f) score += 2; else if (time < 40f) score += 1;
        if (awkward01 < 0.25f) score += 2; else if (awkward01 < 0.6f) score += 1;
        return new[] { "C", "B", "A", "S", "SS" }[Mathf.Clamp(score, 0, 4)];
    }

    void FreezeGameplay()
    {
        foreach (var e in FindObjectsOfType<EnemyController>()) e.Freeze();
        foreach (var f in FindObjectsOfType<FieldOfView2D>()) if (f) f.enabled = false;
        foreach (var m in FindObjectsOfType<DetectionMeter>()) if (m) m.enabled = false;

        var p = FindObjectOfType<PlayerController>();
        if (p)
        {
            var rb = p.GetComponent<Rigidbody2D>(); if (rb) rb.velocity = Vector2.zero;
            p.enabled = false;
        }
    }

    void Clear()
    {
        if (_root == null) return;
        if (Application.isPlaying) Destroy(_root.gameObject);
        else DestroyImmediate(_root.gameObject);
        _root = null;
    }

    void NukeStrays()
    {
        foreach (var p in FindObjectsOfType<PlayerController>()) Destroy(p.gameObject);
        foreach (var e in FindObjectsOfType<EnemyController>()) Destroy(e.gameObject);
        foreach (var c in FindObjectsOfType<Canvas>())
            if (c.renderMode == RenderMode.WorldSpace) Destroy(c.gameObject);
    }

    // ====================== Builders ==============================

    void MakePad(Vector3 pos, int w, int h, char theme, string label, System.Action onSelect)
    {
        var pad = New("Pad", PixelKit.MakeTileSprite(theme, w, h), pos);
        var srb = pad.GetComponent<SpriteRenderer>(); if (srb) srb.sortingOrder = 0;
        var col = pad.AddComponent<BoxCollider2D>(); col.isTrigger = true; col.size = new Vector2(w, h);
        var sp = pad.AddComponent<StandPad>(); sp.Init(label, 1.0f, onSelect);
    }

    GameObject Cover(Vector3 pos, int w, int h)
    {
        var c = New("HideCover", PixelKit.MakeTileSprite('h', w, h), pos);
        var sr = c.GetComponent<SpriteRenderer>(); if (sr) sr.sortingOrder = 0;
        var col = c.AddComponent<BoxCollider2D>(); col.isTrigger = true; col.size = new Vector2(w, h);
        c.AddComponent<HideSpot>();
        MakeWorldText("HIDE", pos + new Vector3(0, 0.85f, 0), 16);
        return c;
    }

    GameObject Distraction(Vector3 pos)
    {
        var d = New("Distraction", PixelKit.MakeTileSprite('c', 1, 1), pos);
        var sr = d.GetComponent<SpriteRenderer>(); if (sr) sr.sortingOrder = 0;
        var col = d.AddComponent<BoxCollider2D>(); col.isTrigger = true;
        d.AddComponent<Distraction>();
        return d;
    }

    GameObject Ex(Vector3 start, Vector3 wp1, Vector3 wp2)
    {
        var go = New("Ex");
        var sr = go.AddComponent<SpriteRenderer>(); var frames = PixelKit.MakeExFrames();
        sr.sprite = frames[0];
        sr.sortingOrder = 5;

        var rb = go.AddComponent<Rigidbody2D>(); rb.gravityScale = 0; rb.bodyType = RigidbodyType2D.Kinematic;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        var anim = go.AddComponent<SpriteAnimator2D>(); anim.idleRunFrames = frames; anim.framesPerSecond = 7f;

        // >>> SNAppier FOV defaults (recognize faster)
        var fov = go.AddComponent<FieldOfView2D>();
        fov.viewRadius = 7.5f;           // longer reach
        fov.viewAngle  = 110f;           // wider cone
        fov.visibilityFactor = 1.1f;     // a bit stronger visibility
        fov.instantChaseDistance = 2.25f;// instant recognition up close
        fov.useVelocityAsForward = true; // cone faces current movement

        var enemy = go.AddComponent<EnemyController>();
        var w1 = new GameObject("Waypoint1").transform; w1.position = wp1; w1.SetParent(_root);
        var w2 = new GameObject("Waypoint2").transform; w2.position = wp2; w2.SetParent(_root);
        enemy.waypoints = new Transform[] { w1, w2 }; enemy.fov = fov;

        go.transform.position = start;
        return go;
    }

    GameObject Friend(Vector3 pos)
    {
        var go = New("FriendLookout");
        var sr = go.AddComponent<SpriteRenderer>(); var frames = PixelKit.MakeFriendFrames();
        sr.sprite = frames[0];
        sr.sortingOrder = 5;

        var anim = go.AddComponent<SpriteAnimator2D>(); anim.idleRunFrames = frames; anim.framesPerSecond = 6f;

        var fov = go.AddComponent<FieldOfView2D>(); // softer cone than Ex (unused for warning now)
        fov.viewRadius = 5.5f; fov.viewAngle = 75f; fov.visibilityFactor = 0.8f;

        go.AddComponent<FriendLookout>();
        go.transform.position = pos;
        return go;
    }

    void Exit(Vector3 pos)
    {
        var e = New("Exit", PixelKit.MakeTileSprite('e', 2, 2), pos);
        var sr = e.GetComponent<SpriteRenderer>(); if (sr) sr.sortingOrder = 0;
        var col = e.AddComponent<BoxCollider2D>(); col.isTrigger = true; col.size = new Vector2(2, 2);
        e.AddComponent<LevelExit>();
        MakeWorldText("EXIT", pos + new Vector3(0, 1.3f, 0), 18);
    }

    void Block(Vector2 center, int wTiles, int hTiles, char theme)
    {
        var w = New("Wall", PixelKit.MakeTileSprite(theme, wTiles, hTiles), new Vector3(center.x, center.y, 0));
        var sr = w.GetComponent<SpriteRenderer>(); if (sr) sr.sortingOrder = 0;
        var col = w.AddComponent<BoxCollider2D>(); col.size = new Vector2(wTiles, hTiles);
    }

    PlayerController SpawnPlayer(Vector3 pos)
    {
        var go = New("Player");
        var sr = go.AddComponent<SpriteRenderer>(); var frames = PixelKit.MakePlayerFrames();
        sr.sprite = frames[0];
        sr.sortingOrder = 5;
        sr.enabled = true;
        sr.color = Color.white;

        var rb = go.AddComponent<Rigidbody2D>(); rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        var col = go.AddComponent<CapsuleCollider2D>(); col.direction = CapsuleDirection2D.Vertical; col.size = new Vector2(0.5f, 0.9f);

        var pc = go.AddComponent<PlayerController>();
        pc.SetHidden(false);

        go.AddComponent<DetectionMeter>();
        var anim = go.AddComponent<SpriteAnimator2D>(); anim.idleRunFrames = frames; anim.framesPerSecond = 8f;
        go.transform.position = pos;

        return pc;
    }

    PlayerController SpawnMenuPlayer(Vector3 pos)
    {
        var go = New("MenuPlayer");
        var sr = go.AddComponent<SpriteRenderer>(); var frames = PixelKit.MakePlayerFrames();
        sr.sprite = frames[0];
        sr.sortingOrder = 5;
        sr.enabled = true; sr.color = Color.white;

        var rb = go.AddComponent<Rigidbody2D>(); rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        var col = go.AddComponent<CapsuleCollider2D>(); col.direction = CapsuleDirection2D.Vertical; col.size = new Vector2(0.5f, 0.9f);

        var pc = go.AddComponent<PlayerController>();
        pc.SetHidden(false);

        var anim = go.AddComponent<SpriteAnimator2D>(); anim.idleRunFrames = frames; anim.framesPerSecond = 8f;
        go.transform.position = pos;
        return pc;
    }

    Camera EnsureCamera()
    {
        var cam = Camera.main;
        if (!cam)
        {
            var go = new GameObject("Main Camera"); cam = go.AddComponent<Camera>();
            go.tag = "MainCamera"; cam.orthographic = true; cam.orthographicSize = 6f; cam.transform.position = new Vector3(0,0,-10);
        }
        if (cam.GetComponent<AudioListener>() == null) cam.gameObject.AddComponent<AudioListener>();
        return cam;
    }

    GameObject New(string name, Sprite sprite = null, Vector3? pos = null)
    {
        if (_root == null) _root = new GameObject("RUNTIME").transform;
        var go = new GameObject(name);
        go.transform.SetParent(_root, false);
        if (sprite != null) { var sr = go.AddComponent<SpriteRenderer>(); sr.sprite = sprite; }
        if (pos.HasValue) go.transform.position = pos.Value;
        return go;
    }

    Text MakeWorldText(string message, Vector3 pos, int size = 24)
    {
        if (_root == null) _root = new GameObject("RUNTIME").transform;

        var canvasGO = new GameObject("WorldText");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 10;

        var rt = canvas.GetComponent<RectTransform>();
        rt.SetParent(_root, true);
        rt.position = pos;
        rt.localScale = Vector3.one * 0.01f;
        rt.sizeDelta = new Vector2(600, 120);

        var textGO = new GameObject("Text");
        textGO.transform.SetParent(canvas.transform, false);
        var t = textGO.AddComponent<Text>();
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.alignment = TextAnchor.MiddleCenter;
        t.color = Color.white;
        t.fontSize = size;
        t.text = message;

        var tr = t.GetComponent<RectTransform>();
        tr.anchorMin = Vector2.zero; tr.anchorMax = Vector2.one; tr.offsetMin = Vector2.zero; tr.offsetMax = Vector2.zero;
        return t;
    }

    // ======================= Backdrops ============================

    void Backdrop_StreetHub()
    {
        var a = New("HubFloor", PixelKit.MakeTileSprite('s', 22, 14), new Vector3(0,0,5));
        var b = New("HubRoad",  PixelKit.MakeTileSprite('a', 12, 4),  new Vector3(0, -1.5f, 4.9f));
        a.GetComponent<SpriteRenderer>().sortingOrder = -5;
        b.GetComponent<SpriteRenderer>().sortingOrder = -5;
    }
    void Backdrop_Cafe()
    {
        var a = New("CafeFloor", PixelKit.MakeTileSprite('v', 22, 14), new Vector3(0,0,5));
        var b = New("Rug",       PixelKit.MakeTileSprite('b', 6, 2),   new Vector3(-2, -2, 4.9f));
        a.GetComponent<SpriteRenderer>().sortingOrder = -5;
        b.GetComponent<SpriteRenderer>().sortingOrder = -5;
    }
    void Backdrop_Campus()
    {
        var a = New("Grass", PixelKit.MakeTileSprite('q', 22, 14), new Vector3(0,0,5));
        var b = New("Walk",  PixelKit.MakeTileSprite('s', 16, 3),  new Vector3(0, -2.5f, 4.9f));
        a.GetComponent<SpriteRenderer>().sortingOrder = -5;
        b.GetComponent<SpriteRenderer>().sortingOrder = -5;
    }
    void Backdrop_Street()
    {
        var a = New("Asphalt",        PixelKit.MakeTileSprite('a', 22, 8), new Vector3(0, -1, 5));
        var b = New("SidewalkTop",    PixelKit.MakeTileSprite('s', 22, 3), new Vector3(0, 5, 4.9f));
        var c = New("SidewalkBottom", PixelKit.MakeTileSprite('s', 22, 3), new Vector3(0, -7, 4.9f));
        a.GetComponent<SpriteRenderer>().sortingOrder = -5;
        b.GetComponent<SpriteRenderer>().sortingOrder = -5;
        c.GetComponent<SpriteRenderer>().sortingOrder = -5;
    }
    void Backdrop_Market()
    {
        var a = New("MarketFloor", PixelKit.MakeTileSprite('v', 22, 14), new Vector3(0,0,5));
        var b = New("MarketRug",   PixelKit.MakeTileSprite('b', 8, 2),   new Vector3(3, -3, 4.9f));
        a.GetComponent<SpriteRenderer>().sortingOrder = -5;
        b.GetComponent<SpriteRenderer>().sortingOrder = -5;
    }
    void Backdrop_Metro()
    {
        var a = New("MetroTrack", PixelKit.MakeTileSprite('a', 22, 6), new Vector3(0, -2.5f, 5));
        var b = New("PlatformTop", PixelKit.MakeTileSprite('s', 22, 4), new Vector3(0,  4.5f, 5));
        var c = New("PlatformBot", PixelKit.MakeTileSprite('s', 22, 4), new Vector3(0, -7.5f, 5));
        a.GetComponent<SpriteRenderer>().sortingOrder = -5;
        b.GetComponent<SpriteRenderer>().sortingOrder = -5;
        c.GetComponent<SpriteRenderer>().sortingOrder = -5;
    }
    void Backdrop_Party()
    {
        var a = New("PartyFloor", PixelKit.MakeTileSprite('b', 22, 14), new Vector3(0,0,5));
        var b = New("Dance",      PixelKit.MakeTileSprite('v', 10, 4),  new Vector3(0, 0, 4.9f));
        a.GetComponent<SpriteRenderer>().sortingOrder = -5;
        b.GetComponent<SpriteRenderer>().sortingOrder = -5;
    }

    // ======================= Caught screen ========================

    void BuildCaught()
    {
        Clear(); PurgeStaminaUI();
        EnsureCamera();
        New("CaughtBG", PixelKit.MakeTileSprite('b', 22, 14), new Vector3(0,0,5))
            .GetComponent<SpriteRenderer>().sortingOrder = -5;

        MakeWorldText("You've been caught!", new Vector3(0, 1.6f, 0), 32);
        MakeWorldText("Awkwardness maxed out. Stand on a pad:", new Vector3(0, 0.8f, 0), 20);

        var menuPlayer = SpawnMenuPlayer(new Vector3(0, -2.6f, 0));

        MakePad(new Vector3(-2.2f, -4.2f, 0), 2, 1, 'g', "RETRY", () =>
        {
            switch (currentLevel)
            {
                case LevelId.Cafe:   BuildLevel_Cafe();   break;
                case LevelId.Campus: BuildLevel_Campus(); break;
                case LevelId.Street: BuildLevel_Street(); break;
                case LevelId.Market: BuildLevel_Market(); break;
                case LevelId.Metro:  BuildLevel_Metro();  break;
                case LevelId.Party:  BuildLevel_Party();  break;
                default:             BuildLevelSelect();  break;
            }
        });

        MakePad(new Vector3( 2.2f, -4.2f, 0), 2, 1, 'g', "LEVEL SELECT", BuildLevelSelect);

        MakeWorldText("R = Quick Restart", new Vector3(0, -5.8f, 0), 16);

        ScreenBarriers();
    }
}
