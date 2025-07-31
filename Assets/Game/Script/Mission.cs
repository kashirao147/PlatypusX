using UnityEngine;
using System.Collections;
namespace PhoenixaStudio
{
	public enum Task
	{
		KillShark,
		DestroyBomb,
		Distance,
		UseRocket,
		UseShield,
		CollectShieldPowerUp,
		CollectMagnetPowerUp,
		CollectBulletPowerUp,
		CollectSpeedBoostPowerUp,
		PlayGame
	}

	[System.Serializable]
	public class Mission
	{

		public Task task;
		[Tooltip("Mission Title")]
		public string mission = "Mission name";
		[Tooltip("Mission Message")]
		public string message = "Information of mission";
		[Tooltip("Target to complete")]
		public int targetAmount = 1;
		[Tooltip("Rewarded coind for complete mission")]
		public int rewardCoin = 10;
	}
}