using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class SmoothSlidingBackground : MonoBehaviour
{
    [Header("Background Settings")]
    public MeshRenderer backgroundRenderer;
    public List<Material> backgroundMaterials;
    public float changeInterval = 30f;   // seconds between transitions
    public float slideDuration = 1.5f;   // how long the slide lasts
    public float slideDistance = 25f;    // how far to move
    public bool slideToLeft = true;      // direction

    private int currentIndex = 0;
    private Material currentMaterial;
    private Coroutine backgroundCycle;

    void Start()
    {
        if (backgroundRenderer == null)
            backgroundRenderer = GetComponent<MeshRenderer>();

        if (backgroundMaterials == null || backgroundMaterials.Count == 0)
        {
            Debug.LogWarning("⚠️ No materials assigned for background!");
            return;
        }

        currentMaterial = backgroundMaterials[currentIndex];
        backgroundRenderer.material = currentMaterial;

       // backgroundCycle = StartCoroutine(AutoChange());
    }

    IEnumerator AutoChange()
    {
        while (true)
        {
            yield return new WaitForSeconds(changeInterval);
           // SlideToNextBackground();
        }
    }

    public void SlideToNextBackground()
    {
        int nextIndex = (currentIndex + 1) % backgroundMaterials.Count;
        Material nextMat = backgroundMaterials[nextIndex];

        // Create temp object for new background
        GameObject newBG = new GameObject("TempBackground");
        var mf = newBG.AddComponent<MeshFilter>();
        var mr = newBG.AddComponent<MeshRenderer>();
        mf.sharedMesh = backgroundRenderer.GetComponent<MeshFilter>().sharedMesh;
        mr.material = nextMat;

        // Match transform
        newBG.transform.SetParent(backgroundRenderer.transform.parent);
        newBG.transform.localScale = backgroundRenderer.transform.localScale;
        newBG.transform.localRotation = backgroundRenderer.transform.localRotation;

        Vector3 startPos = backgroundRenderer.transform.localPosition;
        Vector3 dir = slideToLeft ? Vector3.left : Vector3.right;

        // Set start positions
        newBG.transform.localPosition = startPos + dir * slideDistance;

        // Use DOTween for smooth, eased motion
        backgroundRenderer.transform.DOLocalMove(startPos - dir * slideDistance, slideDuration)
            .SetEase(Ease.InOutQuad);
        newBG.transform.DOLocalMove(startPos, slideDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                // finalize
                backgroundRenderer.material = nextMat;
                backgroundRenderer.transform.localPosition = startPos;
                Destroy(newBG);
                currentIndex = nextIndex;
            });
    }
}
