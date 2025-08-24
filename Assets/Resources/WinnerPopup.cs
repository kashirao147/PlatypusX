using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class WinnerPopup : MonoBehaviour
{
    // Put this script on the root of your Winner screen prefab.
    // Assign all references in the prefab Inspector.

    [Header("UI (assign in prefab)")]
    [SerializeField] private Text titleText;
    [SerializeField] private Text playerANameText;
    [SerializeField] private Text playerAScoreText;
    [SerializeField] private Text playerBNameText;
    [SerializeField] private Text playerBScoreText;
    [SerializeField] private Button okButton;

    [Header("Optional")]
    [Tooltip("If you want me to parent into a specific canvas, set it here. If null, I’ll use/create MessageCanvas.")]
    [SerializeField] private Transform parentCanvas;

    // --------- Static API (one line to show) ---------

    /// <summary>
    /// Instantiates the prefab "Resources/WinnerResultScreenUI" (your prefab with this script),
    /// resolves both player names, sets scores, and waits for OK to close.
    /// </summary>
    /// <param name="res">PlayFabChallengeService.ResolveResult from ResolveChallengeOnReopen</param>
    /// <param name="friendNames">Optional PlayFabId -> DisplayName map</param>
    /// <param name="resourcePath">Your prefab path in Resources (default: WinnerResultScreenUI)</param>
    /// <param name="onOk">Callback after closing</param>
    public static void ShowFromResolve(
        PlayFabChallengeService.ResolveResult res,
        Dictionary<string, string> friendNames = null,
        string resourcePath = "WinnerResultScreenUI",
        Action onOk = null)
    {
        if (res == null || !res.success || res.state != "expired")
        {
            Debug.Log("[WinnerResultScreen] Nothing to show (state != expired).");
            return;
        }

        var prefab = Resources.Load<GameObject>(resourcePath);
        if (!prefab)
        {
            Debug.LogError($"[WinnerResultScreen] Prefab not found at Resources/{resourcePath}");
            return;
        }

        // Choose parent canvas (reuse MessageCanvas if present)
        Transform parent = SelectCanvas();
        var go = Instantiate(prefab, parent);
        var screen = go.GetComponent<WinnerPopup>();
        if (!screen)
        {
            Debug.LogError("[WinnerResultScreen] Script not found on prefab root.");
            return;
        }

        screen.InternalShow(res, friendNames, onOk);
    }

    // --------- Instance logic ---------

    private void InternalShow(PlayFabChallengeService.ResolveResult res,
                              Dictionary<string,string> friendNames,
                              Action onOk)
    {
        var ch = res.challenge;
        if (ch == null)
        {
            Debug.LogWarning("[WinnerResultScreen] Missing challenge in result.");
            Close(onOk);
            return;
        }

        // Order: A = ch.from, B = ch.to (consistent across UI)
        string idA = ch.from;
        string idB = ch.to;

        int scoreA = 0, scoreB = 0;
        if (ch.scores != null)
        {
            if (ch.scores.TryGetValue(idA, out var a)) scoreA = ToInt(a);
            if (ch.scores.TryGetValue(idB, out var b)) scoreB = ToInt(b);
        }
        else
        {
            // Fallback if server didn’t echo per-id scores
            scoreA = res.myScore;
            scoreB = res.oppScore;
        }

        // Resolve names then fill UI and wire OK
        ResolveDisplayName(idA, friendNames, nameA => {
            ResolveDisplayName(idB, friendNames, nameB => {

                string title = scoreA == scoreB
                    ? "It's a Draw"
                    : (scoreA > scoreB ? $"{nameA} Wins!" : $"{nameB} Wins!");

                if (titleText)          titleText.text          ="Challenge Compelte ! "+ title;
                if (playerANameText)    playerANameText.text    = nameA;
                if (playerAScoreText)   playerAScoreText.text   = scoreA.ToString();
                if (playerBNameText)    playerBNameText.text    = nameB;
                if (playerBScoreText)   playerBScoreText.text   = scoreB.ToString();

                if (okButton)
                {
                    okButton.onClick.RemoveAllListeners();
                    okButton.onClick.AddListener(() => Close(onOk));
                }
            });
        });
    }

    private void Close(Action onOk)
    {
        onOk?.Invoke();
        Destroy(gameObject);
    }

    // --------- Helpers ---------

    private static Transform SelectCanvas()
    {
        // If the prefab has its own Canvas at root, Unity will keep it; parenting is still OK.
        var messageCanvas = GameObject.Find("MessageCanvas");
        if (messageCanvas) return messageCanvas.transform;

        var go = new GameObject("MessageCanvas");
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 200;

        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 1;

        go.AddComponent<GraphicRaycaster>();
        return go.transform;
    }

    // Convert possible JSON number box types to int
    private static int ToInt(object v)
    {
        if (v == null) return 0;
        if (v is int i) return i;
        if (v is long l) return (int)l;
        if (v is double d) return (int)Math.Floor(d);
        if (v is float f) return (int)Math.Floor(f);
        if (int.TryParse(v.ToString(), out var x)) return x;
        return 0;
    }

    // Name resolver: friend map -> cached -> PlayFab -> id
    private static readonly Dictionary<string,string> _nameCache = new Dictionary<string,string>();

    private static void ResolveDisplayName(string playFabId,
                                           Dictionary<string,string> friendNames,
                                           Action<string> done)
    {
        if (friendNames != null &&
            friendNames.TryGetValue(playFabId, out var nice) &&
            !string.IsNullOrEmpty(nice))
        { done(nice); return; }

        if (_nameCache.TryGetValue(playFabId, out var cached) &&
            !string.IsNullOrEmpty(cached))
        { done(cached); return; }

        var req = new GetPlayerProfileRequest {
            PlayFabId = playFabId,
            ProfileConstraints = new PlayerProfileViewConstraints { ShowDisplayName = true }
        };
        PlayFabClientAPI.GetPlayerProfile(req, r => {
            var dn = r.PlayerProfile?.DisplayName;
            if (!string.IsNullOrEmpty(dn)) _nameCache[playFabId] = dn;
            done(!string.IsNullOrEmpty(dn) ? dn : playFabId);
        },
        err => {
            Debug.LogWarning("GetPlayerProfile failed: " + err.ErrorMessage);
            done(playFabId);
        });
    }
}
