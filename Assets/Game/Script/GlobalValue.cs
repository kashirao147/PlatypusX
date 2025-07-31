using UnityEngine;
using System.Collections;
namespace PhoenixaStudio
{
	public class GlobalValue : MonoBehaviour
	{
		public static int worldPlaying = 1;
		public static int levelPlaying = 1;

		public static bool openShop = false;

		public static string WorldReached = "WorldReached";

		public static string Points = "Points";
		public static string Character = "Character";

		public static string ChoosenCharacterID = "choosenCharacterID";
		public static string ChoosenCharacterInstanceID = "ChoosenCharacterInstanceID";
		public static GameObject CharacterPrefab;

		public static bool isSound = true;
		public static bool isMusic = true;

		public static int Best
		{
			get { return PlayerPrefs.GetInt("Best", 0); }
			set { PlayerPrefs.SetInt("Best", value); }
		}

		public static int Coin
		{
			get { return PlayerPrefs.GetInt("Coin", 100); }
			set { PlayerPrefs.SetInt("Coin", value); }
		}

		public static bool RemoveAds
		{
			get { return PlayerPrefs.GetInt("RemoveAds", 0) == 1; }
			set { PlayerPrefs.SetInt("RemoveAds", value ? 1 : 0); }
		}

		public static int Rocket
		{
			get { return PlayerPrefs.GetInt("Rocket", 3); }
			set { PlayerPrefs.SetInt("Rocket", value); }
		}

		public static int Bullet
		{
			get { return PlayerPrefs.GetInt("Bullet", 10); }
			set { PlayerPrefs.SetInt("Bullet", value); }
		}

		public static bool isCharacterUnlocked(int ID)
		{
			return PlayerPrefs.GetInt(ID.ToString(), 0) == 1;
		}

		public static void UnlockCharacter(int ID)
		{
			PlayerPrefs.SetInt(ID.ToString(), 1);
		}

		public static int CharacterPicked(int ID, bool isSet)
		{
			if (isSet)
			{
				PlayerPrefs.SetInt("CharacterPicked", ID);
				return 0;
			}
			else
				return PlayerPrefs.GetInt("CharacterPicked", 0);
		}

		public static int HighestMission
		{
			get { return PlayerPrefs.GetInt("HighestMission", 1); }
			set { PlayerPrefs.SetInt("HighestMission", value); }
		}

		/// <summary>
		/// MISSION
		/// </summary>
		/// <value>The shark killed.</value>

		public static int SharkKilled
		{
			get { return PlayerPrefs.GetInt("SharkKilled", 0); }
			set { PlayerPrefs.SetInt("SharkKilled", value); }
		}

		public static bool isStartSharkKilled
		{
			get { return PlayerPrefs.GetInt("isStartSharkKilled", 0) == 1; }
			set { PlayerPrefs.SetInt("isStartSharkKilled", value ? 1 : 0); }
		}

		public static int BombDestroy
		{
			get { return PlayerPrefs.GetInt("BombDestroy", 0); }
			set { PlayerPrefs.SetInt("BombDestroy", value); }
		}

		public static bool isStartBombDestroy
		{
			get { return PlayerPrefs.GetInt("isStartBombDestroy", 0) == 1; }
			set { PlayerPrefs.SetInt("isStartBombDestroy", value ? 1 : 0); }
		}

		public static int UseRocket
		{
			get { return PlayerPrefs.GetInt("UseRocket", 0); }
			set { PlayerPrefs.SetInt("UseRocket", value); }
		}

		public static bool isStartUseRocket
		{
			get { return PlayerPrefs.GetInt("isStartUseRocket", 0) == 1; }
			set { PlayerPrefs.SetInt("isStartUseRocket", value ? 1 : 0); }
		}

		public static int UseShield
		{
			get { return PlayerPrefs.GetInt("UseShield", 0); }
			set { PlayerPrefs.SetInt("UseShield", value); }
		}

		public static bool isStartUseShield
		{
			get { return PlayerPrefs.GetInt("isStartUseShield", 0) == 1; }
			set { PlayerPrefs.SetInt("isStartUseShield", value ? 1 : 0); }
		}

		public static int CollectShieldPowerUp
		{
			get { return PlayerPrefs.GetInt("CollectShieldPowerUp", 0); }
			set { PlayerPrefs.SetInt("CollectShieldPowerUp", value); }
		}

		public static bool isStartCollectShieldPowerUp
		{
			get { return PlayerPrefs.GetInt("isStartCollectShieldPowerUp", 0) == 1; }
			set { PlayerPrefs.SetInt("isStartCollectShieldPowerUp", value ? 1 : 0); }
		}

		public static int CollectMagnetPowerUp
		{
			get { return PlayerPrefs.GetInt("CollectMagnetPowerUp", 0); }
			set { PlayerPrefs.SetInt("CollectMagnetPowerUp", value); }
		}

		public static bool isStartCollectMagnetPowerUp
		{
			get { return PlayerPrefs.GetInt("isStartCollectMagnetPowerUp", 0) == 1; }
			set { PlayerPrefs.SetInt("isStartCollectMagnetPowerUp", value ? 1 : 0); }
		}

		public static int CollectBulletPowerUp
		{
			get { return PlayerPrefs.GetInt("CollectBulletPowerUp", 0); }
			set { PlayerPrefs.SetInt("CollectBulletPowerUp", value); }
		}

		public static bool isStartCollectBulletPowerUp
		{
			get { return PlayerPrefs.GetInt("isStartCollectBulletPowerUp", 0) == 1; }
			set { PlayerPrefs.SetInt("isStartCollectBulletPowerUp", value ? 1 : 0); }
		}

		public static int CollectSpeedBoostPowerUp
		{
			get { return PlayerPrefs.GetInt("CollectSpeedBoostPowerUp", 0); }
			set { PlayerPrefs.SetInt("CollectSpeedBoostPowerUp", value); }
		}

		public static bool isStartCollectSpeedBoostPowerUp
		{
			get { return PlayerPrefs.GetInt("isStartCollectSpeedBoostPowerUp", 0) == 1; }
			set { PlayerPrefs.SetInt("isStartCollectSpeedBoostPowerUp", value ? 1 : 0); }
			
		}

		public static int PlayGame
		{
			get { return PlayerPrefs.GetInt("PlayGame", 0); }
			set { PlayerPrefs.SetInt("PlayGame", value); }
		}
	}
}