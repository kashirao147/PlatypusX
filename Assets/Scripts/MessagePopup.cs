using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessagePopup : MonoBehaviour
{
    public static MessagePopup Instance;
    private static float startingY = 0;
    private static float endingY = 100;
    private static RectTransform activePopup;
  
 

    private void Awake()
    {
        Instance = this;
    }

    public static void ShowPopup(string message, float duration = 1f)
    {
        // AudioManager.Instance.PlayPromptSfx();
        // Destroy existing popup if it exists
        if (activePopup != null)
        {
            Destroy(activePopup.gameObject);
            activePopup = null;
        }

        // Kill any existing tweens with this ID
        DOTween.Kill("msg-popup");

        // Create a new popup
      
        RectTransform container = Instantiate(Resources.Load<RectTransform>("MessagePopupUI"), SelectCanvas());
        activePopup = container;
        CanvasGroup canvasGroup = container.GetComponent<CanvasGroup>();
        TextMeshProUGUI messageText = container.GetComponentInChildren<TextMeshProUGUI>();

        messageText.text = message;
        container.anchoredPosition = new Vector2(0f, startingY);
        canvasGroup.alpha = 0f;

        // Animate the popup
        DOTween.Sequence()
            .Append(container.DOAnchorPosY(endingY, 0.5f))
            .Join(canvasGroup.DOFade(1f, 0.15f))
            .AppendInterval(duration)
            .Append(canvasGroup.DOFade(0f, 0.15f))
            .SetId("msg-popup")
            .OnComplete(() => 
            {
                Destroy(container.gameObject);
                activePopup = null;
            });
    }

    private static Transform SelectCanvas() {
        if (GameObject.Find("MessageCanvas") == null) {
            GameObject canvasObj = new GameObject("MessageCanvas");
            Canvas canvas = canvasObj.AddComponent(typeof(Canvas)) as Canvas;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            CanvasScaler canvasScaler = canvasObj.AddComponent(typeof(CanvasScaler)) as CanvasScaler;
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            canvasScaler.matchWidthOrHeight = 1;
            canvasObj.AddComponent(typeof(GraphicRaycaster));
            return canvasObj.transform;
        }
        else {
            return GameObject.Find("MessageCanvas").transform;
        }
    }
}