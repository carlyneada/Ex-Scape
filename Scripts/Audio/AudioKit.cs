// Assets/Scripts/Audio/AudioKit.cs
using UnityEngine;

public static class AudioKit
{
    // Cached clips so we only synthesize once.
    static AudioClip _hideClip;
    static AudioClip _fakeClip;
    static AudioClip _pingClip;

    const int SAMPLE_RATE = 44100;

    // Small helper: Mathf doesn't have Tanh.
    static float Tanhf(float x) => (float)System.Math.Tanh(x);

    /// Play when the player successfully hides.
    public static void PlayHide(Vector3 pos, float volume = 0.85f)
    {
        if (_hideClip == null) _hideClip = MakeHideClip();
        AudioSource.PlayClipAtPoint(_hideClip, pos, Mathf.Clamp01(volume));
    }

    /// Play when the player triggers a fake call (Q).
    public static void PlayFakeCall(Vector3 pos, float volume = 0.95f)
    {
        if (_fakeClip == null) _fakeClip = MakeFakeCallClip();
        AudioSource.PlayClipAtPoint(_fakeClip, pos, Mathf.Clamp01(volume));
    }

    /// Small alert ping (used by FriendLookout when you’re being chased).
    public static void PlayPing(Vector3 pos, float volume = 0.8f)
    {
        if (_pingClip == null) _pingClip = MakePingClip();
        AudioSource.PlayClipAtPoint(_pingClip, pos, Mathf.Clamp01(volume));
    }

    // ---------- Synth helpers ----------

    static AudioClip MakeHideClip()
    {
        // Cute descending "bloop": 0.14s sweep ~900Hz -> ~650Hz with a short decay tail.
        float dur = 0.14f;
        int count = (int)(dur * SAMPLE_RATE);
        var data = new float[count];

        float startHz = 900f;
        float endHz   = 650f;

        for (int n = 0; n < count; n++)
        {
            float t = n / (float)SAMPLE_RATE;
            float k = n / (float)count;                  // 0..1
            float hz = Mathf.Lerp(startHz, endHz, k);
            float env = Mathf.Pow(1f - k, 0.25f);        // fast decay
            float s = Mathf.Sin(2f * Mathf.PI * hz * t);
            data[n] = Tanhf(s * 0.85f) * env * 0.7f;
        }
        var clip = AudioClip.Create("ak_hide", count, 1, SAMPLE_RATE, false);
        clip.SetData(data, 0);
        return clip;
    }

    static AudioClip MakeFakeCallClip()
    {
        // Phone-y trill: 3 very short beeps at 1200 Hz with tiny gaps.
        float beepDur = 0.06f;
        float gapDur  = 0.035f;
        int beepSamples = (int)(beepDur * SAMPLE_RATE);
        int gapSamples  = (int)(gapDur  * SAMPLE_RATE);

        int total = (beepSamples + gapSamples) * 3 - gapSamples; // last one no trailing gap
        var data = new float[total];

        int idx = 0;
        for (int b = 0; b < 3; b++)
        {
            // Beep
            for (int n = 0; n < beepSamples; n++, idx++)
            {
                float t = n / (float)SAMPLE_RATE;
                float env = 1f - (n / (float)beepSamples); // quick decay
                float s = Mathf.Sin(2f * Mathf.PI * 1200f * t);
                data[idx] = Tanhf(s * 0.9f) * env * 0.7f;
            }
            // Gap (silence) except after last beep
            if (b < 2)
            {
                for (int n = 0; n < gapSamples; n++, idx++) data[idx] = 0f;
            }
        }

        var clip = AudioClip.Create("ak_fakecall", total, 1, SAMPLE_RATE, false);
        clip.SetData(data, 0);
        return clip;
    }

    static AudioClip MakePingClip()
    {
        // Sharp UI ping: 1600 Hz with very fast decay, ~0.08s
        float dur = 0.08f;
        int count = (int)(dur * SAMPLE_RATE);
        var data = new float[count];

        for (int n = 0; n < count; n++)
        {
            float t = n / (float)SAMPLE_RATE;
            float env = Mathf.Pow(1f - (n / (float)count), 0.12f);
            float s = Mathf.Sin(2f * Mathf.PI * 1600f * t);
            data[n] = Tanhf(s) * env * 0.9f;
        }
        var clip = AudioClip.Create("ak_ping", count, 1, SAMPLE_RATE, false);
        clip.SetData(data, 0);
        return clip;
    }
}
