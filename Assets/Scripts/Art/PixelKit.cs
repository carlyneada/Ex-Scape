using System.Collections.Generic;
using UnityEngine;

/// Tiny 16x16 pixel sprites built at runtime (FilterMode.Point).
public static class PixelKit
{
    public const int PPU  = 16;  // pixels per unit for sprites
    public const int SIZE = 16;  // sprite width/height
    public static readonly Color Clear = new Color(0,0,0,0);

    static Dictionary<char, Color> _palette;
    static Dictionary<char, Color> Palette {
        get {
            if (_palette != null) return _palette;
            _palette = new Dictionary<char, Color>();
            // Character colors
            _palette['f'] = new Color32(255,229,200,255); // face
            _palette['b'] = new Color32(255,170,170,255); // blush
            _palette['h'] = new Color32(60,40,30,255);    // hair
            _palette['o'] = new Color32(30,20,20,255);    // outline
            _palette['e'] = new Color32(240,220,90,255);  // exit yellow (light) for sprites
            _palette['E'] = new Color32(210,190,70,255);  // exit yellow (dark)  for sprites
            _palette['w'] = new Color32(255,255,255,255); // white
            _palette['p'] = new Color32(120,220,180,255); // pastel green
            _palette['P'] = new Color32(90,180,150,255);  // darker green
            _palette['x'] = new Color32(230,70,90,255);   // red
            _palette['X'] = new Color32(190,40,60,255);   // dark red
            _palette['r'] = new Color32(255,190,70,255);  // orange
            _palette['R'] = new Color32(220,150,50,255);  // dark orange
            _palette['d'] = new Color32(70,90,110,255);   // denim
            _palette['s'] = new Color32(30,30,30,255);    // shoes/black
            _palette[' '] = new Color(0,0,0,0);           // empty
            return _palette;
        }
    }

    // === PUBLIC BUILDERS ===
    public static Sprite[] MakePlayerFrames() => MakeCharacterFrames(PlayerIdle, PlayerRun1, PlayerRun2);
    public static Sprite[] MakeExFrames()     => MakeCharacterFrames(ExIdle, ExRun1, ExRun2);
    public static Sprite[] MakeFriendFrames() => MakeCharacterFrames(FriendIdle, FriendRun1, FriendRun2);

    /// World/background tiles (checker-dithered blocks).
    /// theme legend:
    /// 'g' = green pad (select), 'e' = yellow (EXIT), 'c' = blue (distraction),
    /// 'h' = purple (HIDE cover), 'w' = wall, 's' = sidewalk, 'a' = asphalt,
    /// 'v' = cafe wood, 'q' = campus grass, 'b' = splash/backdrop.
    public static Sprite MakeTileSprite(char theme, int wTiles, int hTiles)
    {
        Color a, b;
        switch (theme)
        {
            case 'g': a = new Color32( 90,200, 90,255); b = new Color32( 70,170, 70,255); break;
            case 'e': a = new Color32(240,220, 90,255); b = new Color32(210,190, 70,255); break;
            case 'c': a = new Color32( 86,146,200,255); b = new Color32( 70,120,170,255); break;
            case 'h': a = new Color32(170,120,220,255); b = new Color32(150,100,200,255); break; // HIDE tiles (distinct purple)
            case 'w': a = new Color32( 85, 85, 85,255); b = new Color32( 70, 70, 70,255); break;
            case 's': a = new Color32(205,205,205,255); b = new Color32(185,185,185,255); break;
            case 'a': a = new Color32( 65, 65, 70,255); b = new Color32( 55, 55, 60,255); break;
            case 'v': a = new Color32(200,170,120,255); b = new Color32(180,150,105,255); break;
            case 'q': a = new Color32(120,180,120,255); b = new Color32(100,160,100,255); break;
            case 'b': a = new Color32(120,165,210,255); b = new Color32(100,145,190,255); break;
            default : a = new Color32(110,110,110,255); b = new Color32( 95, 95, 95,255); break;
        }
        int w = wTiles * SIZE, h = hTiles * SIZE;
        var tex = NewTexture(w, h);
        for (int y=0; y<h; y++)
        for (int x=0; x<w; x++)
            tex.SetPixel(x, y, (((x/4)+(y/4))%2==0) ? a : b);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0,0,w,h), new Vector2(0.5f,0.5f), PPU);
    }

    /// Tiny 16x16 red exclamation icon (used above friend lookout)
    public static Sprite MakeIconExclamation()
    {
        string[] rows = new string[SIZE] {
            "                ",
            "      xx        ",
            "      xx        ",
            "      xx        ",
            "      xx        ",
            "      xx        ",
            "      xx        ",
            "      xx        ",
            "      xx        ",
            "      xx        ",
            "      xx        ",
            "                ",
            "      xx        ",
            "      xx        ",
            "                ",
            "                ",
        };
        return MakeSprite(rows);
    }

    // === INTERNAL HELPERS ===
    static Sprite[] MakeCharacterFrames(string[] f0, string[] f1, string[] f2)
        => new Sprite[] { MakeSprite(f0), MakeSprite(f1), MakeSprite(f2) };

    static Sprite MakeSprite(string[] rows)
    {
        var tex = NewTexture(SIZE, SIZE);
        for (int y=0; y<SIZE; y++)
        {
            string row = rows[y];
            for (int x=0; x<SIZE; x++)
            {
                char key = (x < row.Length) ? row[x] : ' ';
                if (!Palette.TryGetValue(key, out var c)) c = Clear;
                tex.SetPixel(x, SIZE-1-y, c);
            }
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0,0,SIZE,SIZE), new Vector2(0.5f,0.1f), PPU);
    }

    static Texture2D NewTexture(int w, int h)
    {
        var tex = new Texture2D(w, h, TextureFormat.ARGB32, false);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode   = TextureWrapMode.Clamp;
        return tex;
    }

    // ===== SPRITE PIXEL MAPS =====
    // Player
    static readonly string[] PlayerIdle = {
"       oooo      ","     oohhhhoo    ","    ohhffffhho   ","   ohfwe  ewfo   ",
"   ohf  ee  fo   ","   ohf b  b fo   ","    ohfffffho    ","    oohPPPPoo    ",
"     oPPPPPo     ","    pppPPPPpp    ","    pppPPPPpp    ","     ddddddd     ",
"     d     d     ","     s     s     ","                  ","                  ",
    };
    static readonly string[] PlayerRun1 = {
"       oooo      ","     oohhhhoo    ","    ohhffffhho   ","   ohfwe  ewfo   ",
"   ohf  ee  fo   ","    ohfffffho    ","    oohPPPPoo    ","   pppPPPPpp     ",
"   pppPPPPpp     ","     ddddddd     ","    d   d        ","    s   s        ",
"                  ","                  ","                  ","                  ",
    };
    static readonly string[] PlayerRun2 = {
"       oooo      ","     oohhhhoo    ","    ohhffffhho   ","   ohfwe  ewfo   ",
"   ohf  ee  fo   ","    ohfffffho    ","    oohPPPPoo    ","     oPPPPPo     ",
"    pppPPPPpp    ","    pppPPPPpp    ","       d   d     ","       s   s     ",
"                  ","                  ","                  ","                  ",
    };

    // Ex
    static readonly string[] ExIdle = {
"       oooo      ","     oohhhhoo    ","    ohhffffhho   ","   ohf eee ef o  ",
"   ohf  e  f o   ","    ohfffffho    ","    ooxXXXXoo    ","     oXXXXxo     ",
"    dddXXXXddd   ","    dddXXXXddd   ","     ddddddd     ","     d     d     ",
"     s     s     ","                  ","                  ","                  ",
    };
    static readonly string[] ExRun1 = {
"       oooo      ","     oohhhhoo    ","    ohhffffhho   ","   ohf eee ef o  ",
"   ohf  e  f o   ","    ohfffffho    ","    ooxXXXXoo    ","   dddXXXXddd    ",
"   dddXXXXddd    ","     ddddddd     ","    d   d        ","    s   s        ",
"                  ","                  ","                  ","                  ",
    };
    static readonly string[] ExRun2 = {
"       oooo      ","     oohhhhoo    ","    ohhffffhho   ","   ohf eee ef o  ",
"   ohf  e  f o   ","    ohfffffho    ","    ooxXXXXoo    ","     oXXXXxo     ",
"    dddXXXXddd   ","    dddXXXXddd   ","       d   d     ","       s   s     ",
"                  ","                  ","                  ","                  ",
    };

    // Friend
    static readonly string[] FriendIdle = {
"       oooo      ","     oohhhhoo    ","    ohhffffhho   ","   ohf  ee  fo   ",
"   ohf beeb fo   ","    ohfffffho    ","    oorRRRRoo    ","     oRRRRRo     ",
"    dddRRRRddd   ","    dddRRRRddd   ","     ddddddd     ","     d     d     ",
"     s     s     ","                  ","                  ","                  ",
    };
    static readonly string[] FriendRun1 = {
"       oooo      ","     oohhhhoo    ","    ohhffffhho   ","   ohf  ee  fo   ",
"   ohf beeb fo   ","    ohfffffho    ","    oorRRRRoo    ","   dddRRRRddd    ",
"   dddRRRRddd    ","     ddddddd     ","    d   d        ","    s   s        ",
"                  ","                  ","                  ","                  ",
    };
    static readonly string[] FriendRun2 = {
"       oooo      ","     oohhhhoo    ","    ohhffffhho   ","   ohf  ee  fo   ",
"   ohf beeb fo   ","    ohfffffho    ","    oorRRRRoo    ","     oRRRRRo     ",
"    dddRRRRddd   ","    dddRRRRddd   ","       d   d     ","       s   s     ",
"                  ","                  ","                  ","                  ",
    };
}
