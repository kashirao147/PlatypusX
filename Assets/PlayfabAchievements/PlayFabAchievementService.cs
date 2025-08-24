using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using PlayFab.Json;
public static class PlayFabAchievementService
{
    [Serializable] public class AchDef {
        public string id;
        public string type;     // "trophy" or "title"
        public int coins;
        public string sprite;   // sprite name you’ll map in UI
    }
    [Serializable] public class ReportResult {
        public bool success;
        public int totalCoins;
        public string[] newAchievements;
        public NextTarget nextTarget;
    }
    [Serializable] public class OverviewResult {
        public bool success;
        public int totalCoins;
        public List<AchDef> defs;
        public List<string> unlocked;
        public List<string> pending;
        public NextTarget nextTarget;
    }
    [Serializable] public class AckResult {
        public bool success;
        public List<string> removed;
        public List<string> pending;
        public string message;
    }
    [Serializable] public class NextTarget { public string id; public string type; public int coins; }

    // --- helpers ---
    private static T As<T>(ExecuteCloudScriptResult r)
    {
        var json = PlayFabSimpleJson.SerializeObject(r.FunctionResult);
        return PlayFabSimpleJson.DeserializeObject<T>(json);
    }

     private static OverviewResult _overviewCache;
    private static float _overviewCacheAt;
    private const float CacheTtlSec = 2f;

    private static bool _overviewInFlight;
    private static readonly List<Action<OverviewResult>> _overviewQueue = new();

    // private static T As<T>(ExecuteCloudScriptResult r)
    // {
    //     var json = PlayFab.SimpleJson.SimpleJson.SerializeObject(r.FunctionResult);
    //     if (string.IsNullOrEmpty(json) || json == "null") return default(T);
    //     return PlayFab.SimpleJson.SimpleJson.DeserializeObject<T>(json);
    // }

    public static void InvalidateAchievementsCache()
    {
        _overviewCache = null;
        _overviewCacheAt = 0f;
    }

    public static void GetAchievementsOverview(
        Action<OverviewResult> done,
        Action<PlayFabError> fail = null,
        bool useCache = true)
    {
        // quick cache
        if (useCache && _overviewCache != null &&
            (Time.realtimeSinceStartup - _overviewCacheAt) < CacheTtlSec)
        {
            done?.Invoke(_overviewCache);
            return;
        }

        // coalesce concurrent calls
        _overviewQueue.Add(done);
        if (_overviewInFlight) return;
        _overviewInFlight = true;

        var req = new ExecuteCloudScriptRequest {
            FunctionName = "GetAchievementsOverview",
            GeneratePlayStreamEvent = false
        };

        PlayFabClientAPI.ExecuteCloudScript(req, r =>
        {
            _overviewInFlight = false;
            var res = (r.Error != null)
                ? new OverviewResult { success = false }
                : (As<OverviewResult>(r) ?? new OverviewResult { success = false });

            if (r.Error != null)
                Debug.LogError($"CloudScript error (GetAchievementsOverview): {r.Error.Error} - {r.Error.Message}");

            if (res.success)
            {
                _overviewCache = res;
                _overviewCacheAt = Time.realtimeSinceStartup;
            }

            foreach (var cb in _overviewQueue) cb?.Invoke(res);
            _overviewQueue.Clear();

        }, e =>
        {
            _overviewInFlight = false;
            fail?.Invoke(e);
            var res = new OverviewResult { success = false };
            foreach (var cb in _overviewQueue) cb?.Invoke(res);
            _overviewQueue.Clear();
        });
    }

    // Call this after you *change* achievements on the server:
    public static void ReportCoinsAndEvaluate(int coinsEarned, Action<ReportResult> done, Action<PlayFabError> fail = null)
    {
        var req = new ExecuteCloudScriptRequest {
            FunctionName = "ReportCoinsAndEvaluate",
            FunctionParameter = new Dictionary<string, object> { { "coinsEarned", coinsEarned } },
            GeneratePlayStreamEvent = false
        };
        PlayFabClientAPI.ExecuteCloudScript(req, r =>
        {
            var res = (r.Error != null) ? new ReportResult { success = false } : (As<ReportResult>(r) ?? new ReportResult { success = false });
            if (r.Error != null)
                Debug.LogError($"CloudScript error (ReportCoinsAndEvaluate): {r.Error.Error} - {r.Error.Message}");
            // invalidate cache because totals/unlocks may have changed
            InvalidateAchievementsCache();
            done?.Invoke(res);
        }, e => fail?.Invoke(e));
    }

    public static void AckAchievementNotifications(IEnumerable<string> ids, Action<AckResult> done, Action<PlayFabError> fail = null)
    {
        var req = new ExecuteCloudScriptRequest {
            FunctionName = "AckAchievementNotifications",
            FunctionParameter = new Dictionary<string, object> { { "ids", new List<string>(ids) } },
            GeneratePlayStreamEvent = false
        };
        PlayFabClientAPI.ExecuteCloudScript(req, r =>
        {
            var res = (r.Error != null) ? new AckResult { success = false } : (As<AckResult>(r) ?? new AckResult { success = false });
            if (r.Error != null)
                Debug.LogError($"CloudScript error (AckAchievementNotifications): {r.Error.Error} - {r.Error.Message}");
            // pending changed → invalidate
            InvalidateAchievementsCache();
            done?.Invoke(res);
        }, e => fail?.Invoke(e));
    }
}
