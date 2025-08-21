using UnityEngine;
using TMPro;
using PhoenixaStudio;
using UnityEngine.UI;

public class InGameMissionUIController : MonoBehaviour
{
    [Header("UI References")]
    public Text missionTypeText;     // e.g. "Mission: Shield"
    public Text descriptionText;     // e.g. "Use 5 shields"
    public Text remainingText;       // e.g. "Remaining: 3"


    void Start()
    {
        RefreshUI();
    }




    public void RefreshUI()
    {
        Mission mission = MissionManager.Instance.Missions[MissionManager.Instance.MissionUI[0].MissionID];
        float left = Mathf.Clamp(mission.targetAmount - MissionValue(mission.task), 0, int.MaxValue);
        if (left == 0)
        {
            missionTypeText.text = mission.mission + " Complete";
        }

        if (missionTypeText) missionTypeText.text = $"Mission: {mission.mission}";
        if (descriptionText) descriptionText.text = mission.message;


        if (remainingText) remainingText.text = $"Remaining: {left}";
    }

    public float MissionValue(Task missionTask)
    {
        switch (missionTask)
				{
					case Task.KillShark:
                      return GlobalValue.SharkKilled;
						

					case Task.DestroyBomb:
                      return GlobalValue.BombDestroy;
						
					case Task.Distance:
                     return (int)GameManager.Instance.distance;
						

					case Task.UseRocket:
                        return GlobalValue.UseRocket;
						

					case Task.UseShield:
                     return GlobalValue.UseShield;
						

					case Task.CollectShieldPowerUp:
                     return GlobalValue.CollectShieldPowerUp;
						

					case Task.CollectMagnetPowerUp:
                     return GlobalValue.CollectMagnetPowerUp;
						

					case Task.CollectBulletPowerUp:
                     return GlobalValue.CollectBulletPowerUp;
						
					case Task.CollectSpeedBoostPowerUp:
                     return GlobalValue.CollectSpeedBoostPowerUp;
						

					case Task.PlayGame:
                        return GlobalValue.PlayGame;
						
						
					default:
						return 0f;
				}

    }
}
