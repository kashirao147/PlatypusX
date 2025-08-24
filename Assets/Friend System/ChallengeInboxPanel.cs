using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChallengeInboxPanel : MonoBehaviour
{

    // references from your scene
    public GameObject activePanel;
    public Text opponentText;
    public Text myScoreText;
    public Text oppScoreText;
    public Button quitButton;
    

    



    [SerializeField] private Transform content;                 // ScrollRect content
    [SerializeField] private ChallengeRequestRow rowPrefab;     // your prefab with the script above

    // Optional map: PlayFabId -> DisplayName (fill this from your friends list)
    public Dictionary<string, string> FriendNames = new Dictionary<string, string>();

    private readonly List<GameObject> _spawned = new List<GameObject>();

    private void OnEnable() { Refresh(); }

    public void Refresh()
    {
        // call whenever you open the screen or need a refresh
        // call whenever you open the screen (and after each round if you want to refresh numbers)
        PlayFabChallengeService.ShowActiveWithScoresAndQuit(
            opponentText,
            myScoreText,
            oppScoreText,
            quitButton,
            activePanel,
  
            
            onQuitDone: () => {
                Debug.Log("Quit done, refreshed active HUD.");
            });


        // clear old
        foreach (var go in _spawned) Destroy(go);
        _spawned.Clear();

        // load incoming
       PlayFabChallengeService.ListChallenges("incoming", res =>
        {
            if (!res.success || res.incoming == null) return;

            foreach (var ch in res.incoming)
            {
                var go  = Instantiate(rowPrefab.gameObject, content);
                var row = go.GetComponent<ChallengeRequestRow>();

                // start with an immediate fallback (id or map hit)
                var initialName = (FriendNames != null && FriendNames.TryGetValue(ch.from, out var n) && !string.IsNullOrEmpty(n))
                                    ? n : ch.from;

                row.Setup(ch, initialName, onChanged: Refresh, toast: (m) => Debug.Log(m));
                _spawned.Add(go);

                // then resolve properly (fetch from PlayFab if needed) and update the UI
                PlayFabChallengeService.ResolveDisplayName(ch.from, FriendNames, resolvedName =>
                {
                    row.SetSenderName(resolvedName);
                    // also cache into your FriendNames map for later
                    if (FriendNames != null) FriendNames[ch.from] = resolvedName;
                });
            }
        },
        err => Debug.LogError(err.GenerateErrorReport()));
    }
}
