using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewAchievementPopup : MonoBehaviour
{
    [Header("Prefab override (optional)")]
    public static GameObject PrefabOverride;  // assign once at boot if you prefer Inspector

    public Image slotBG;
    public Image icon;
    public TMP_Text titleText;
    public TMP_Text labelText;
    public Button okButton;

    private System.Func<string, Sprite> _resolve;

    private static readonly string[] SearchPaths = {
        "NewAchievementPopup",
        "UI/NewAchievementPopup",
        "UI/Achievements",
        "Popups/NewAchievementPopup",
        "Achievements/NewAchievementPopup"
    };

    private static GameObject LoadPrefab()
    {
        if (PrefabOverride) return PrefabOverride;

        // try common resource paths
        foreach (var p in SearchPaths)
        {
            var pf = Resources.Load<GameObject>(p);
            if (pf) { Debug.Log($"[NewAchievementPopup] Loaded prefab: Resources/{p}"); return pf; }
        }

#if UNITY_EDITOR
        // last-resort debug scan (loads all, not for production)
        foreach (var go in Resources.LoadAll<GameObject>(""))
            if (go && go.name == "NewAchievementPopup")
                return go;
#endif
        return null;
    }

    public static void ShowOne(string achievementId,
                               List<PlayFabAchievementService.AchDef> defs,
                               System.Func<string, Sprite> resolve,
                               System.Action onOK)
    {
        var prefab = LoadPrefab();
        if (!prefab) {
            Debug.LogError("NewAchievementPopup prefab not found. Put it in Assets/Resources/[optional subfolders]/NewAchievementPopup.prefab " +
                           "or assign NewAchievementPopup.PrefabOverride at startup.");
            return;
        }

        var go = Instantiate(prefab, SelectCanvas());
        var pop = go.GetComponent<NewAchievementPopup>();
        if (!pop) { Debug.LogError("NewAchievementPopup component missing on prefab root."); Destroy(go); return; }
        pop._resolve = resolve;

        var def = defs?.Find(d => d.id == achievementId);
        pop.Fill(def);

        if (pop.okButton)
        {
            pop.okButton.onClick.RemoveAllListeners();
            pop.okButton.onClick.AddListener(() => {
                Destroy(go);
                onOK?.Invoke();
            });
        }
    }

    private void Fill(PlayFabAchievementService.AchDef def)
    {
       // if (slotBG) slotBG.sprite = _resolve?.Invoke("trophy_slot_unlocked_1"); // or your fancy bg
        if (icon && def != null && !string.IsNullOrEmpty(def.sprite))
        {
            var s = _resolve(def.sprite);
            if (s) { icon.sprite = s; icon.enabled = true; }
        }
        if (titleText) titleText.text = "You earned "; // you can localize by def.id
        if (labelText) labelText.text = "On completeing "+def.coins; // "Unlocked!" if you want
    }

    private static Transform SelectCanvas()
    {
        var go = GameObject.Find("MessageCanvas");
        if (go) return go.transform;
        var c = new GameObject("MessageCanvas");
        var canvas = c.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 300;
        
        c.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        c.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
        c.GetComponent<CanvasScaler>().matchWidthOrHeight=1;
        c.AddComponent<GraphicRaycaster>();
        return c.transform;
    }
}
