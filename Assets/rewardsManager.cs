using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NiobiumStudios;
public class rewardsManager : MonoBehaviour
{
    private void Start()
    {
        
    }
    void OnEnable()
    {
       // DailyRewards.instance.onClaimPrize += OnClaimPrizeDailyRewards;
    }

    void OnDisable()
    {
       // DailyRewards.instance.onClaimPrize -= OnClaimPrizeDailyRewards;
    }

    // this is your integration function. Can be on Start or simply a function to be called
    public void OnClaimPrizeDailyRewards(int day)
    {
        //This returns a Reward object
        Reward myReward = DailyRewards.instance.GetReward(day);

        // And you can access any property
        print(myReward.unit);   // This is your reward Unit name
        print(myReward.reward); // This is your reward count

        var rewardsCount = PlayerPrefs.GetInt("Coin", 0);
        rewardsCount += myReward.reward;

        PlayerPrefs.SetInt("Coin", rewardsCount);
        PlayerPrefs.Save();
    }
    }
