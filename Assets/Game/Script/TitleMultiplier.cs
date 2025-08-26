using System;
using System.Collections.Generic;
using PhoenixaStudio;

public static class TitleMultiplier
{
    // Adjust values as you like.
    private static readonly Dictionary<string, float> Map =
        new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase)
        {
            { "rookie",   1.00f },
            { "runner",   1.10f },
            { "sprinter", 1.20f },
            { "turbo",    1.30f },
            { "dasher",   1.40f },
            { "legend",   1.60f }
        };

    /// <summary>
    /// Returns the score multiplier based on the player's current title sprite.
    /// If the title is missing/unknown, returns 1f.
    /// </summary>
    public static float Get()
    {
        string sprite = GlobalValue.GetPlayerTitle(); // e.g., "rookie", "runner", etc.
        if (string.IsNullOrWhiteSpace(sprite))
            return 1f;

        sprite = sprite.Trim();
        return Map.TryGetValue(sprite, out float mult) ? mult : 1f;
    }

    /// <summary>
    /// Optional: query by an explicit sprite string.
    /// </summary>
    public static float Get(string titleSprite)
    {
        if (string.IsNullOrWhiteSpace(titleSprite))
            return 1f;

        return Map.TryGetValue(titleSprite.Trim(), out float mult) ? mult : 1f;
    }
}
