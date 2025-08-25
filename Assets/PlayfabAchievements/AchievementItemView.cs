using PhoenixaStudio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementItemView : MonoBehaviour
{
    [Header("Refs")]
    public Image slotBG;
    public Image icon;
    public GameObject lockOverlay;
    public TMP_Text nameText;
    public TMP_Text targetText;

    [Header("Sprites")]
    public Sprite slotLockedSprite;    // trophy_slot_locked_x
    public Sprite slotUnlockedSprite;  // trophy_slot_unlocked_x (generic ok)
    // You’ll map def.sprite → Sprite via your own atlas loader or references

    // Provide a lookup from sprite name to Sprite (set from the screen)
    private System.Func<string, Sprite> _spriteResolver;

    public void Setup(PlayFabAchievementService.AchDef def, bool isUnlocked, int totalCoins,
                      System.Func<string, Sprite> spriteResolver,
                      System.Func<string, string> nameLocalizer = null)
    {
        _spriteResolver = spriteResolver;
        if(isUnlocked && def.id.Contains("title")){
            GlobalValue.SetPlayerTitle(def.sprite);
        }

        // BG state
        if (slotBG) slotBG.sprite = isUnlocked ? slotUnlockedSprite : slotLockedSprite;

        // Icon from def.sprite (if present)
        if (icon && spriteResolver != null)
        {
            var s = spriteResolver(def.sprite);
            if (s) { icon.sprite = s; icon.enabled = true; }
            else   { icon.enabled = false; }
        }

        // Lock overlay / target coins
        if (lockOverlay) lockOverlay.SetActive(!isUnlocked);
        if (targetText) targetText.text = isUnlocked ? "" : $"{def.coins:n0}";

        // Optional name text (you can localize by id; else leave blank)
        if (nameText) nameText.text = (nameLocalizer != null) ? nameLocalizer(def.id) : "";
    }
}
