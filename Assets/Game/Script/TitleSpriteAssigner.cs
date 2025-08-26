using System;
using System.Collections.Generic;
using PhoenixaStudio;
using UnityEngine;
using UnityEngine.UI;

public class TitleSpriteAssigner : MonoBehaviour
{
    [Serializable]
    public class TitleSprite
    {
        [Tooltip("Title key (usually the sprite/name string like: rookie, runner, sprinter, turbo, dasher, legend).")]
        public string titleKey;

        [Tooltip("Sprite to use when the player's title matches titleKey.")]
        public Sprite sprite;
    }

    [Header("Assign ONE target (auto-find if left null)")]
    public Image uiImage;                 // For UI
    public SpriteRenderer spriteRenderer; // For world sprite

    [Header("Title → Sprite map")]
    public List<TitleSprite> titleSprites = new List<TitleSprite>();

    [Header("Fallback")]
    public Sprite defaultSprite;          // Used if no match found (optional)
    public bool autoAssignOnEnable = true;
 
    // Fast lookup
    private Dictionary<string, Sprite> _map;

    void Awake()
    {
        if (uiImage == null) uiImage = GetComponent<Image>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        BuildMap();
    }

    void OnEnable()
    {
        if (autoAssignOnEnable) Refresh();
    }

    /// <summary>Call this when title may have changed at runtime.</summary>
    public void Refresh()
    {
        // e.g., "rookie", "runner", "sprinter", "turbo", "dasher", "legend"
        string currentTitle = GlobalValue.GetPlayerTitle();
        AssignByTitle(currentTitle);
    }

    /// <summary>Assign by explicit title key (case-insensitive).</summary>
    public void AssignByTitle(string titleKey)
    {
        if (_map == null) BuildMap();

        Sprite s = null;
        if (!string.IsNullOrWhiteSpace(titleKey))
        {
            var k = Normalize(titleKey);
            _map.TryGetValue(k, out s);
        }

        if (s == null) s = defaultSprite;
        ApplySprite(s);
    }

    private void ApplySprite(Sprite s)
    {
        if (uiImage != null) uiImage.sprite = s;
        if (spriteRenderer != null) spriteRenderer.sprite = s;
    }

    private void BuildMap()
    {
        _map = new Dictionary<string, Sprite>(StringComparer.OrdinalIgnoreCase);
        foreach (var ts in titleSprites)
        {
            if (ts == null || ts.sprite == null || string.IsNullOrWhiteSpace(ts.titleKey))
                continue;

            var key = Normalize(ts.titleKey);
            _map[key] = ts.sprite; // last wins if duplicates
        }
    }

    private static string Normalize(string s)
    {
        // case-insensitive, trim, collapse spaces/underscores
        s = s.Trim();
        s = s.Replace(" ", "").Replace("_", "");
        return s.ToLowerInvariant();
    }
}
