using System;
using System.Linq;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

[Serializable]
public class GlobalEventData
{
    public string id;
    public string title;
    public string body;
    public string timestampIso;
}

public static class GlobalEventService
{
    // PlayerPrefs keys
    private const string PREF_JSON     = "GlobalEvent.JSON";
    private const string PREF_HASH     = "GlobalEvent.Hash";
    private const string PREF_LASTSEEN = "GlobalEvent.LastSeenHash";

    // Optional: only treat news whose Title starts with this marker as “global events”
    // Set to "" to accept any news item as the global event.
    public static string EventTitleMarker = "[EVENT]";

    /// Fetch latest title news (client-side), cache newest “event”.
    /// - If a *new* event is found (hash differs), it’s written to PlayerPrefs and `isNew=true`.
    public static void FetchAndCacheLatest(Action<bool, string, bool> done, int count = 10)
    {
        PlayerPrefs.DeleteKey(PREF_JSON);
        PlayerPrefs.DeleteKey(PREF_HASH);
        PlayerPrefs.DeleteKey(PREF_LASTSEEN);
        var req = new GetTitleNewsRequest { Count = count };
        PlayFabClientAPI.GetTitleNews(req, res =>
        {
            if (res?.News == null || res.News.Count == 0)
            {
                done?.Invoke(false, "No title news found.", false);
                return;
            }

            // pick the newest item (PlayFab returns newest first, but sort to be safe)
            var items = res.News
            .OrderByDescending(n => n.Timestamp)   // Timestamp is DateTime
            .ToList();

            // filter by marker if set
            TitleNewsItem chosen = null;
            if (!string.IsNullOrEmpty(EventTitleMarker))
                chosen = items.FirstOrDefault(n => (n.Title ?? "").StartsWith(EventTitleMarker, StringComparison.OrdinalIgnoreCase));

            if (chosen == null) chosen = items.First(); // fallback: take the newest news as event

            var ts = chosen.Timestamp;                 // DateTime
        if (ts == default(DateTime)) ts = DateTime.UtcNow; // safety if SDK ever returns default
        var data = new GlobalEventData {
            id           = chosen.NewsId,
            title        = chosen.Title ?? "",
            body         = chosen.Body  ?? "",
            timestampIso = ts.ToUniversalTime().ToString("o")
        };

            var newHash = ComputeHash(data);
            var oldHash = PlayerPrefs.GetString(PREF_HASH, "");
            var isNew   = newHash != oldHash;

            // Always cache the latest (even if same)
            PlayerPrefs.SetString(PREF_JSON, JsonUtility.ToJson(data));
            PlayerPrefs.SetString(PREF_HASH, newHash);
            PlayerPrefs.Save();

            done?.Invoke(true, null, isNew);
        },
        err =>
        {
            done?.Invoke(false, err?.GenerateErrorReport(), false);
        });
    }

    /// Load cached event (or null if none)
    public static GlobalEventData LoadCached()
    {
        var json = PlayerPrefs.GetString(PREF_JSON, "");
        if (string.IsNullOrEmpty(json)) return null;
        try { return JsonUtility.FromJson<GlobalEventData>(json); }
        catch { return null; }
    }

    /// Has the user already seen the current cached event?
    public static bool IsCachedEventNew()
    {
        var cur = PlayerPrefs.GetString(PREF_HASH, "");
        var seen = PlayerPrefs.GetString(PREF_LASTSEEN, "");
        return !string.IsNullOrEmpty(cur) && cur != seen;
    }

    /// Mark the current cached event as “seen” so it won’t auto-pop next time.
    public static void MarkSeen()
    {
        var cur = PlayerPrefs.GetString(PREF_HASH, "");
        if (!string.IsNullOrEmpty(cur))
        {
            PlayerPrefs.SetString(PREF_LASTSEEN, cur);
            PlayerPrefs.Save();
        }
    }

    /// Handy getters
    public static string GetCachedTitle() => LoadCached()?.title ?? "";
    public static string GetCachedBody()  => LoadCached()?.body  ?? "";

    private static string ComputeHash(GlobalEventData d)
    {
        var s = (d?.title ?? "") + "|" + (d?.body ?? "") + "|" + (d?.timestampIso ?? "");
        // simple hash; good enough for change detection
        return s.GetHashCode().ToString("X8");
    }
}
