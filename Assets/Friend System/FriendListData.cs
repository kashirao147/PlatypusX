using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab;

using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using PhoenixaStudio;

public class FriendListData : MonoBehaviour
{
    List<FriendInfo> _friends = null;


    [SerializeField] InputField playerid;
    [SerializeField] Transform ParrentTransformdisplayFriend;
    [SerializeField] GameObject FriendCardItem;
    [SerializeField] Text PlayerID;



    #region Copy to clipboard id
      #if UNITY_IOS && !UNITY_EDITOR
            [DllImport("__Internal")] private static extern void SetClipboardText(string str);
        #endif

            /// <summary>Copies current text from the assigned input field.</summary>
            public void CopyFieldText()
            {
                string text = GetFieldText();
                if (string.IsNullOrEmpty(text)) return;
                Copy(text);
            }

            /// <summary>Copy any string programmatically.</summary>
            public static void Copy(string text)
            {
                if (string.IsNullOrEmpty(text)) return;

        #if UNITY_IOS && !UNITY_EDITOR
                SetClipboardText(text);
        #else
                GUIUtility.systemCopyBuffer = text; // Android / PC / macOS / Editor
        #endif
            }

            private string GetFieldText()
            {
                if (PlayerID != null)  return PlayerID.text;
                
                return "";
            }


    #endregion






    private void OnEnable()
    {
        PlayerID.text = GlobalValue.getMyPlayfabID();
        getfriendlist();
    }

   

    public void getfriendlist()
    {
        PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest
        {
            // ExternalPlatformFriends =null

        }, result =>
        {
            _friends = result.Friends;
            DisplayFriends(_friends); // triggers your UI
        }, DisplayPlayFabError);
    }



    void DisplayFriends(List<FriendInfo> friendsCache)
    {
        foreach (Transform child in ParrentTransformdisplayFriend)
            Destroy(child.gameObject);

        friendsCache.ForEach(f =>
        {
            GameObject newCard = Instantiate(FriendCardItem, ParrentTransformdisplayFriend);

            var card = newCard.GetComponent<FriendCard>();
            if (card == null) card = newCard.AddComponent<FriendCard>(); // safety

            // Initialize and wire the challenge button
            card.Init(f, OnChallengeClicked);
        });
    }

    private void OnChallengeClicked(string friendPlayFabId)
    {
        PlayFabChallengeService.SendChallenge(friendPlayFabId, res => {
            if (res.success) MessagePopup.ShowPopup("Challenge sent: " + res.challenge.id);
            else MessagePopup.ShowPopup("Send failed: " + res.message);
        }, err => Debug.LogError(err.GenerateErrorReport()));
        
        // // Open your Create Challenge screen and pass the id
        // if (createChallengeScreen != null)
        // {
        //     // If you want to also show the friendly name, you can look it up from the current list.
        //     var info = _friends?.Find(x => x.FriendPlayFabId == friendPlayFabId);
        //     var friendlyName = (info != null && !string.IsNullOrEmpty(info.TitleDisplayName))
        //                         ? info.TitleDisplayName
        //                         : friendPlayFabId;

        //     createChallengeScreen.Open(friendPlayFabId, friendlyName);
        // }
        // else
        // {
        //     Debug.Log($"[FriendList] Challenge pressed for: {friendPlayFabId} (no screen hooked yet)");
        // }
    }

    void DisplayPlayFabError(PlayFabError error) { Debug.Log(error.GenerateErrorReport()); }
    void DisplayError(string error) { Debug.LogError(error); }



    public void addfriend()
    {
        // AddFriendCall(playerid.text);
        ExecuteButton(playerid.text);
    }


  







    #region

   
    public void ExecuteButton(string friendId)
    {

        var req = new ExecuteCloudScriptRequest
        {

            FunctionName = "SendFriendRequest",
            FunctionParameter = new
            {
                FriendPlayFabId = friendId
            }
        };
        PlayFabClientAPI.ExecuteCloudScript(req, OnSuccessExecute, OnError);
    }





    void OnSuccessExecute(ExecuteCloudScriptResult result)
    {
        if (result.FunctionResult != null)
        {
            // Deserialize the JSON result into our C# class
            var json = result.FunctionResult.ToString();
            var response = JsonConvert.DeserializeObject<FriendRequestResponse>(json);
            MessagePopup.ShowPopup(response.message);
            // Now you can use the values
            if (response.success)
            {
                Debug.Log("Friend added: " + response.to);

            }
            else
            {
                Debug.LogWarning("Friend request failed: " + response.message);

            }
        }
        else
        {
            Debug.LogWarning("No FunctionResult returned from cloud script.");
            //RequestReportaddFriend.text = "Unexpected error";
        }

        getfriendlist();

        
    }

    void OnError(PlayFabError error)
    {
        MessagePopup.ShowPopup(error.GenerateErrorReport());
        Debug.Log("Login Error : " + error.GenerateErrorReport());

    }
    

    #endregion
}
    [System.Serializable]
    public class FriendRequestResponse
    {
        public bool success;
        public string message;
        public string from;
        public string to;
        public string state;
    }
