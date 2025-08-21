using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace PhoenixaStudio
{
	public class MissionManager : MonoBehaviour
	{
		public static MissionManager Instance;
		public AudioClip soundRewarded;
		public AudioClip soundMissionComplete;
		[Header("Mission UI")]
		public Mission_UI[] MissionUI;
		[Header("Mission Task")]
		public Mission[] Missions;

		int missionAssigned = 0;
		void Awake()
		{
			Instance = this;
			//first disable all the mission UI
			foreach (var UI in MissionUI)
				UI.RewardButton.transform.parent.gameObject.SetActive(false);
			//Check and display the current mission information
			for (int i = 0; i < Missions.Length; i++)
			{
				if (!CheckMissionCompleted(i) && !isTaskStypeExist(i))
				{
					MissionUI[missionAssigned].RewardButton.transform.parent.gameObject.SetActive(true);
					MissionUI[missionAssigned].MissionID = i;
					MissionUI[missionAssigned].missionNumber.text = GlobalValue.HighestMission == 1 ? missionAssigned + 1 + "" : GlobalValue.HighestMission + missionAssigned + "";
					MissionUI[missionAssigned].missionNameTxt.text = Missions[i].mission;
					MissionUI[missionAssigned].missionMessageTxt.text = Missions[i].message;
					MissionUI[missionAssigned].missionTargetTxt.text = Missions[i].targetAmount.ToString();
					MissionUI[missionAssigned].missonRewardCoin.text = Missions[i].rewardCoin.ToString();
					MissionUI[missionAssigned].RewardButton.SetActive(false);
					ResetValueForMission(Missions[i].task);
					missionAssigned++;
					if (missionAssigned >= MissionUI.Length)
						break;
				}
			}
		}

		//Check if the mission task is available in the list
		private bool isTaskStypeExist(int ID)
		{
			if (missionAssigned > 0)
			{
				foreach (var mission in MissionUI)
				{
					if (Missions[mission.MissionID].task == Missions[ID].task && mission.RewardButton.transform.parent.gameObject.activeSelf)
					{
						return true;
					}
				}
			}

			return false;
		}

		//Reset the value of the mission task
		private void ResetValueForMission(Task task)
		{
			switch (task)
			{
				case Task.KillShark:
					if (!GlobalValue.isStartSharkKilled)
					{
						GlobalValue.SharkKilled = 0;
						GlobalValue.isStartSharkKilled = true;
					}
					break;

				case Task.DestroyBomb:
					if (!GlobalValue.isStartBombDestroy)
					{
						GlobalValue.BombDestroy = 0;
						GlobalValue.isStartBombDestroy = true;
					}
					break;

				case Task.Distance:

					break;

				case Task.UseRocket:
					if (!GlobalValue.isStartUseRocket)
					{
						GlobalValue.UseRocket = 0;
						GlobalValue.isStartUseRocket = true;
					}
					break;

				case Task.UseShield:
					if (!GlobalValue.isStartUseShield)
					{
						GlobalValue.UseShield = 0;
						GlobalValue.isStartUseShield = true;
					}
					break;

				case Task.CollectShieldPowerUp:
					if (!GlobalValue.isStartCollectShieldPowerUp)
					{
						GlobalValue.CollectShieldPowerUp = 0;
						GlobalValue.isStartCollectShieldPowerUp = true;
					}
					break;

				case Task.CollectMagnetPowerUp:
					if (!GlobalValue.isStartCollectMagnetPowerUp)
					{
						GlobalValue.CollectMagnetPowerUp = 0;
						GlobalValue.isStartCollectMagnetPowerUp = true;
					}
					break;

				case Task.CollectBulletPowerUp:
					if (!GlobalValue.isStartCollectBulletPowerUp)
					{
						GlobalValue.CollectBulletPowerUp = 0;
						GlobalValue.isStartCollectBulletPowerUp = true;
					}
					break;

				case Task.CollectSpeedBoostPowerUp:
					if (!GlobalValue.isStartCollectSpeedBoostPowerUp)
					{
						GlobalValue.CollectSpeedBoostPowerUp = 0;
						GlobalValue.isStartCollectSpeedBoostPowerUp = true;
					}
					break;

				case Task.PlayGame:

					break;

				default:
					break;
			}
		}

		//call this when gameover
		public void CheckMissions()
		{
			int left;
			foreach (var mission in MissionUI)
			{
				switch (Missions[mission.MissionID].task)
				{
					case Task.KillShark:
						if (mission.RewardButton.transform.parent.gameObject.activeSelf)
						{
							left = Mathf.Clamp(Missions[mission.MissionID].targetAmount - GlobalValue.SharkKilled, 0, int.MaxValue);
							mission.missionTargetTxt.text = left > 0 ? "Remain: " + left : "Mission Complete";
							if (left == 0)
							{
								MissionComplete(mission.MissionID);
								mission.RewardButton.SetActive(true);
								GlobalValue.isStartSharkKilled = false;
							}
						}
						break;

					case Task.DestroyBomb:
						if (mission.RewardButton.transform.parent.gameObject.activeSelf)
						{
							left = Mathf.Clamp(Missions[mission.MissionID].targetAmount - GlobalValue.BombDestroy, 0, int.MaxValue);
							mission.missionTargetTxt.text = left > 0 ? "Remain: " + left : "Mission Complete";
							if (left == 0)
							{
								MissionComplete(mission.MissionID);
								mission.RewardButton.SetActive(true);
								GlobalValue.isStartBombDestroy = false;
							}
						}
						break;
					case Task.Distance:
						if (mission.RewardButton.transform.parent.gameObject.activeSelf)
						{
							left = Mathf.Clamp((Missions[mission.MissionID].targetAmount - (int)GameManager.Instance.distance), 0, int.MaxValue);
							if (left == 0)
							{
								mission.missionTargetTxt.text = "Mission Complete";
								MissionComplete(mission.MissionID);
								mission.RewardButton.SetActive(true);
							}
							else
								mission.missionTargetTxt.text = "reached: " + (int)GameManager.Instance.distance;
						}
						break;

					case Task.UseRocket:
						if (mission.RewardButton.transform.parent.gameObject.activeSelf)
						{
							left = Mathf.Clamp(Missions[mission.MissionID].targetAmount - GlobalValue.UseRocket, 0, int.MaxValue);
							mission.missionTargetTxt.text = left > 0 ? "Remain: " + left : "Mission Complete";
							if (left == 0)
							{
								MissionComplete(mission.MissionID);
								mission.RewardButton.SetActive(true);
								GlobalValue.isStartUseRocket = false;
							}
						}
						break;

					case Task.UseShield:
						if (mission.RewardButton.transform.parent.gameObject.activeSelf)
						{
							left = Mathf.Clamp(Missions[mission.MissionID].targetAmount - GlobalValue.UseShield, 0, int.MaxValue);
							mission.missionTargetTxt.text = left > 0 ? "Remain: " + left : "Mission Complete";
							if (left == 0)
							{
								MissionComplete(mission.MissionID);
								mission.RewardButton.SetActive(true);
								GlobalValue.isStartUseShield = false;
							}
						}
						break;

					case Task.CollectShieldPowerUp:
						if (mission.RewardButton.transform.parent.gameObject.activeSelf)
						{
							left = Mathf.Clamp(Missions[mission.MissionID].targetAmount - GlobalValue.CollectShieldPowerUp, 0, int.MaxValue);
							mission.missionTargetTxt.text = left > 0 ? "Remain: " + left : "Mission Complete";
							if (left == 0)
							{
								MissionComplete(mission.MissionID);
								mission.RewardButton.SetActive(true);
								GlobalValue.isStartCollectShieldPowerUp = false;
							}
						}
						break;

					case Task.CollectMagnetPowerUp:
						if (mission.RewardButton.transform.parent.gameObject.activeSelf)
						{
							left = Mathf.Clamp(Missions[mission.MissionID].targetAmount - GlobalValue.CollectMagnetPowerUp, 0, int.MaxValue);
							mission.missionTargetTxt.text = left > 0 ? "Remain: " + left : "Mission Complete";
							if (left == 0)
							{
								MissionComplete(mission.MissionID);
								mission.RewardButton.SetActive(true);
								GlobalValue.isStartCollectMagnetPowerUp = false;
							}
						}
						break;

					case Task.CollectBulletPowerUp:
						if (mission.RewardButton.transform.parent.gameObject.activeSelf)
						{
							left = Mathf.Clamp(Missions[mission.MissionID].targetAmount - GlobalValue.CollectBulletPowerUp, 0, int.MaxValue);
							mission.missionTargetTxt.text = left > 0 ? "Remain: " + left : "Mission Complete";
							if (left == 0)
							{
								MissionComplete(mission.MissionID);
								mission.RewardButton.SetActive(true);
								GlobalValue.isStartCollectBulletPowerUp = false;
							}
						}
						break;

					case Task.CollectSpeedBoostPowerUp:
						if (mission.RewardButton.transform.parent.gameObject.activeSelf)
						{
							left = Mathf.Clamp(Missions[mission.MissionID].targetAmount - GlobalValue.CollectSpeedBoostPowerUp, 0, int.MaxValue);
							mission.missionTargetTxt.text = left > 0 ? "Remain: " + left : "Mission Complete";
							if (left == 0)
							{
								MissionComplete(mission.MissionID);
								mission.RewardButton.SetActive(true);
								GlobalValue.isStartCollectSpeedBoostPowerUp = false;
							}
						}
						break;

					case Task.PlayGame:
						if (mission.RewardButton.transform.parent.gameObject.activeSelf)
						{
							left = Mathf.Clamp(Missions[mission.MissionID].targetAmount - GlobalValue.PlayGame, 0, int.MaxValue);
							mission.missionTargetTxt.text = left > 0 ? "Remain: " + left : "Mission Complete";
							if (left == 0)
							{
								MissionComplete(mission.MissionID);
								mission.RewardButton.SetActive(true);
							}
						}
						break;
					default:
						break;
				}

			}

			if (isAnyTaskCompleted())
				SoundManager.PlaySfx(soundMissionComplete);
		}

		public bool isAnyTaskCompleted()
		{
			foreach (var mission in MissionUI)
			{
				if (mission.RewardButton.activeInHierarchy)
					return true;
			}
			return false;
		}

		//Buttons
		public void MissionButtonI()
		{
			if (!MissionUI[0].RewardButton.activeInHierarchy)
				return;

			GlobalValue.Coin += Missions[MissionUI[0].MissionID].rewardCoin;
			SoundManager.PlaySfx(soundRewarded);
			MissionUI[0].RewardButton.SetActive(false);
		}

		public void MissionButtonII()
		{
			if (!MissionUI[1].RewardButton.activeInHierarchy)
				return;

			GlobalValue.Coin += Missions[MissionUI[1].MissionID].rewardCoin;
			SoundManager.PlaySfx(soundRewarded);
			MissionUI[1].RewardButton.SetActive(false);
		}

		public void MissionButtonIII()
		{
			if (!MissionUI[2].RewardButton.activeInHierarchy)
				return;

			GlobalValue.Coin += Missions[MissionUI[2].MissionID].rewardCoin;
			SoundManager.PlaySfx(soundRewarded);
			MissionUI[2].RewardButton.SetActive(false);
		}

		//Set and Get mission information
		private void MissionComplete(int ID)
		{
			GlobalValue.HighestMission++;
			PlayerPrefs.SetInt(ID.ToString(), 1);
		}

		private bool CheckMissionCompleted(int ID)
		{
			return PlayerPrefs.GetInt(ID.ToString(), 0) == 1;
		}

		public void GetAllRewarded()
		{
			MissionButtonI();
			MissionButtonII();
			MissionButtonIII();
		}

		[System.Serializable]
		public class Mission_UI
		{
			public int MissionID { get; set; }
			public Text missionNumber;
			public Text missionNameTxt;
			public Text missionMessageTxt;
			public Text missionTargetTxt;
			public Text missonRewardCoin;
			public GameObject RewardButton;
		}
	}
}