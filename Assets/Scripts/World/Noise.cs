using UnityEngine;
using System;

public static class Noise
{
    public static event Action<Vector2> OnNoise;
    public static void Emit(Vector2 position) => OnNoise?.Invoke(position);
}
