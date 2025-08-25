using System.Collections.Generic;
using PhoenixaStudio;
using UnityEngine;

public class LevelMenuController : MonoBehaviour
{
    [System.Serializable]
    public class LevelItem
    {
        [Tooltip("Root image GameObject for this level (e.g., Level1 image).")]
        public GameObject root;

        [Tooltip("Optional: the child that indicates 'active/selected'. If left empty, the first child will be used.")]
        public Transform activeChild;
    }

    [Header("Order must be: Level1, Level2, Level3")]
    public List<LevelItem> levelItems = new List<LevelItem>(3);

    private void OnEnable()
    {
        RefreshUI();
    }

    public void SelectLevel(string Level)
    {
        GlobalValue.setSelectedLevel(Level);
        RefreshUI();
    }

    /// <summary>
    /// Call this if unlocks/selection change while the menu is open.
    /// </summary>
    public void RefreshUI()
    {
        // e.g., "Level1", "Level2", "Level3"
        string selectedLevel = GlobalValue.getSelectedLevel();

        // We only handle up to 3 levels as requested
        int count = Mathf.Min(levelItems.Count, 3);

        for (int i = 0; i < count; i++)
        {
            string levelName = "Level" + (i + 1);
            LevelItem item = levelItems[i];

            if (item.root == null)
                continue;

            bool isUnlocked = GlobalValue.isUnlockLevel(levelName);

            // Show only unlocked images
            item.root.SetActive(isUnlocked);
            if (!isUnlocked)
                continue;

            bool isSelected = !string.IsNullOrEmpty(selectedLevel) && selectedLevel == levelName;

            // Determine which child acts as the "active" indicator
            Transform activeChild = item.activeChild;
            if (activeChild == null && item.root.transform.childCount > 0)
            {
                // Default to first child if none specified
                activeChild = item.root.transform.GetChild(0);
            }

            // Turn on the active indicator only for the selected level.
            // All other child objects should be off.
            int childCount = item.root.transform.childCount;
            for (int c = 0; c < childCount; c++)
            {
                Transform child = item.root.transform.GetChild(c);
                bool shouldBeActive = isSelected && activeChild != null && child == activeChild;
                child.gameObject.SetActive(shouldBeActive);
            }
        }
    }

#if UNITY_EDITOR
    // Handy button in the inspector to test in edit mode
    [ContextMenu("Refresh UI")]
    private void _EditorRefresh() => RefreshUI();
#endif
}
