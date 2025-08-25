using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementsScreen : MonoBehaviour
{
    [Header("Top UI")]
    public Image panelBG;
    public Image headerBanner;
    public Text headerText;
    public Image progressBG;
    public Image progressFill;
    public TMP_Text nextTargetText;
    public Image coinIcon;
    public Button closeButton;

    [Header("Grid")]
    public ScrollRect scroll;
    public Transform content;              // GridLayoutGroup
    public AchievementItemView itemPrefab;

    [Header("Atlas/Sprites")]
    public List<Sprite> sprites;           // drag your sliced sprites here
    // Map sprite.name → Sprite
    private Dictionary<string, Sprite> _spriteMap;

    private void Awake()
    {
        _spriteMap = new Dictionary<string, Sprite>();
        foreach (var s in sprites) if (s) _spriteMap[s.name] = s;
        if (closeButton) closeButton.onClick.AddListener(() => gameObject.SetActive(false));
    }

    private Sprite Resolve(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        return _spriteMap.TryGetValue(name, out var s) ? s : null;
    }

    public void Open()
    {
        gameObject.SetActive(true);
        Refresh();
    }

    public void Refresh()
    {
        // Clear old
        for (int i = content.childCount - 1; i >= 0; i--) Destroy(content.GetChild(i).gameObject);

        PlayFabAchievementService.GetAchievementsOverview(ov =>
        {
            if (ov == null || !ov.success) { Debug.LogWarning("Overview failed"); return; }
            Debug.Log($"defs:{ov?.defs?.Count} unlocked:{ov?.unlocked?.Count} coins:{ov?.totalCoins} next:{ov?.nextTarget?.coins}");
            if (ov?.defs != null)
                foreach (var d in ov.defs) Debug.Log($"DEF {d.id} coins={d.coins} sprite={d.sprite}");
        });


        PlayFabAchievementService.GetAchievementsOverview(ov =>
        {
            if (ov == null || !ov.success) { Debug.LogWarning("Overview failed"); return; }
            if (!ov.success) { return; }

            // Progress
           // if (progressBG) progressBG.sprite = Resolve("progress_empty");
            

            int total = ov.totalCoins;
            int next  = ov.nextTarget != null ? ov.nextTarget.coins : total;
            float pct = (ov.nextTarget == null || next <= 0) ? 1f : Mathf.Clamp01((float)total / next);
            if (progressFill) {
                // var s = Resolve("progress_fill");
                // if (s) progressFill.sprite = s;
                // var rt = progressFill.rectTransform;
                // var w = (progressBG ? progressBG.rectTransform.rect.width : 600f);
                progressFill.fillAmount = pct;
                //rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w * pct);
            }
            if (nextTargetText) nextTargetText.text = (ov.nextTarget != null) ? $"Next: {next:n0}" : "Maxed";

            // Build items
            var unlockedSet = new HashSet<string>(ov.unlocked ?? new List<string>());
            foreach (var def in ov.defs)
            {
                
                // if (ov.nextTarget.coins == def.coins)
                // {
                //     if (coinIcon)   coinIcon.sprite   = Resolve(def.sprite);
                // }
                var item = Instantiate(itemPrefab, content);
                // choose generic locked/unlocked bg sprites you added to the prefab
                item.Setup(def, unlockedSet.Contains(def.id), ov.totalCoins, Resolve);
            }

        }, err => Debug.LogError(err.GenerateErrorReport()));
    }
}
