using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using PhoenixaStudio;

public class PlayFabManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static PlayFabManager Instance;
    public GameObject NameWindow;
   // public GameObject LeaderboardWindow;
    public InputField nameInput;
    public InputField emailInput;
    public GameObject rowPrefab;
    public Transform rowParrent;
    public bool isLogin=false;
    public static string DeviceUniqueIdentifier
    {
        get
        {
            var deviceId = "";


#if UNITY_EDITOR
            deviceId = SystemInfo.deviceUniqueIdentifier + "-editor";
            Debug.Log("its editor");
#elif UNITY_ANDROID
                AndroidJavaClass up = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject> ("currentActivity");
                AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject> ("getContentResolver");
                AndroidJavaClass secure = new AndroidJavaClass ("android.provider.Settings$Secure");
                deviceId = secure.CallStatic<string> ("getString", contentResolver, "android_id");
#elif UNITY_WEBGL
                if (!PlayerPrefs.HasKey("UniqueIdentifier"))
                    PlayerPrefs.SetString("UniqueIdentifier", System.Guid.NewGuid().ToString());
                deviceId = PlayerPrefs.GetString("UniqueIdentifier");
#else
                deviceId = SystemInfo.deviceUniqueIdentifier;
#endif
            return deviceId;
        }
    }
    void Start()
    {
        Login();    
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = DeviceUniqueIdentifier, CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
    };
        PlayFabClientAPI.LoginWithCustomID(request, OnSuccess, OnError);
    }

    public void OnSuccess(LoginResult result)
    {
        GlobalValue.setMyPlayfabID(result.PlayFabId);
        isLogin = true;
        PlayFabAchievementService.ReportCoinsAndEvaluate(0,null);
       // On resume / after login
        PlayFabChallengeService.ResolveChallengeOnReopen(res => {
            if (!res.success) return;
            //Global Events
            GlobalEventService.FetchAndCacheLatest((ok, err, isNew) =>
            {
                if (!ok)
                {
                    Debug.LogWarning("Title News fetch failed: " + err);
                    return;
                }

                // Optional: auto-show only if it changed since last time:
                if (isNew || GlobalEventService.IsCachedEventNew())
                {
                    // Use your own popup UI (MessagePopup, custom panel, etc.)
                    // Example using your MessagePopup:
                  //  MessagePopup.ShowPopup(GlobalEventService.GetCachedBody());

                    // Or enable a banner:
                    // globalEventBannerGameObject.SetActive(true);

                    //GlobalEventService.MarkSeen();
                }
            });







            if (res.state == "expired")
            {

                WinnerPopup.ShowFromResolve(
                    res,
                    null,                 // optional: PlayFabId -> DisplayName
                    "WinnerScreen",      // your prefab path in Resources
                    onOk: () =>
                    {
                        // anything you want after closing (e.g., refresh UI)
                    }
                );
                //MessagePopup.ShowPopup($"Challenge over. You {res.outcome}. My:{res.myScore} Opp:{res.oppScore}");
                // Now your side is cleared; opponent will clear when they call this too.
            }
            else if (res.state == "active")
            {
                Debug.Log($"Still active. My:{res.myScore} Opp:{res.oppScore}");
            }
            else
            {
                Debug.Log("No active challenge.");
            }
        }, err => Debug.LogError(err.GenerateErrorReport()));




        //  PlayFabChallengeService.CheckChallengeOnReopen(res =>
        // {
        //     if (!res.success) return;

        //     switch (res.state)
        //     {
        //         case "expired":
        //             MessagePopup.ShowPopup("Challenge is complete");
        //             Debug.Log("Challenge is complete (time over). ID: " + res.challenge.id);
        //             // TODO: refresh UI (hide active panel, refresh inbox, etc.)
        //             break;

        //         case "active":
        //             Debug.Log("Challenge still active vs: " + (res.challenge.from));
        //             break;

        //         case "none":
        //             Debug.Log("No active challenge.");
        //             break;
        //     }
        // },
        // err => Debug.LogError(err.GenerateErrorReport()));

        Debug.Log("Sccessfuly login/account created!");
        string name = null;
        if (result.InfoResultPayload.PlayerProfile != null)
        name = result.InfoResultPayload.PlayerProfile.DisplayName;
        if (name == null)
        {
            if (GlobalValue.getCanShowNamePannel())
            {
                GlobalValue.setCanShowNamePannel(0);
                NameWindow.SetActive(true);
            }
        }
        PlayerPrefs.SetString("NameLB", name);

    }

    public void OnError(PlayFabError error)
    {
        Debug.Log("Error while creating/loging in account!");
        Debug.Log(error.GenerateErrorReport());
    }

    public void submitName()
    {
        string name=nameInput.text;
        if (nameInput.text == "" || string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
        {
            MessagePopup.ShowPopup("Please Enter Name!");
            return;
        }
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = nameInput.text
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, OnError);
        NameWindow.SetActive(false);
        PlayerPrefs.SetString("NameLB", nameInput.text);
        submitEmail();
    }
    public void submitEmail()
    {
        var request = new AddOrUpdateContactEmailRequest
        {
            EmailAddress = emailInput.text
        };
        PlayFabClientAPI.AddOrUpdateContactEmail(request, OnDisplayEmailUpdate, OnError);
    }
    void OnDisplayEmailUpdate(AddOrUpdateContactEmailResult result)
    {
        Debug.Log("Updated Email!");
    }
    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Updated Display Name!");
    } 
    public void sendLeaderboard(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "Score",Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, onLeaderboardUpdate, OnError);
    }

     void onLeaderboardUpdate (UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Leadbaord Updated Sucessfully!");
    }

    public void getLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "Score",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(request, onLeadboardGet, OnError);
    }

    void onLeadboardGet(GetLeaderboardResult result)
    {
        foreach (Transform item in rowParrent)
        {
            Destroy(item.gameObject);
        }
        foreach (var item in result.Leaderboard)
            {
            GameObject newGO = Instantiate(rowPrefab, rowParrent);
            Text[] texts = newGO.GetComponentsInChildren<Text>();
            texts[0].text = (item.Position +1).ToString();
            texts[1].text = item.DisplayName;
            texts[2].text = item.StatValue.ToString();

            Debug.Log(item.Position + " " + item.PlayFabId + " " + item.StatValue);
            }
    }
}
