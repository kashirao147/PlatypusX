using UnityEngine;
using System.Collections;
namespace PhoenixaStudio
{
	public class GameManager : MonoBehaviour
	{
		public static GameManager Instance;

		public enum GameState { Menu, Playing, Pause, GameOver }
        public PlayFabManager playfab;
		public GameState State { get; set; }
		[Header("=== TEST LEVEL ===")]
		public bool spawnTestLevel = false;
		public GameObject testLevelItem;
		public float testSpeed = 9;
		[Header("=== GAME LEVEL ===")]
		public GameObject levelEmpty;
		public LevelBlocks[] LevelBlock;

		[Header("Floating Text")]
		public GameObject FloatingText;
		private MainMenu menuManager;
		//the speed of the submarine
		public float Speed { get; set; }
		public int Score { get; set; }
		[HideInInspector]
		public float distance = 0;

        private const string DeathCountKey = "deathCount"; // PlayerPrefs key
        private const string RemoveAdsKey = "RemoveAds"; // Key for ad removal purchase

        public GameObject RocketHolder { get; set; }
		public GameObject BulletHolder { get; set; }
		public Player Player { get
			{
				if (playerTemp != null) return playerTemp;
				else
				{
					playerTemp = FindObjectOfType<Player>();
					return playerTemp;

				};
			}
			set { playerTemp = value; }
		}
			
		public SoundManager SoundManager { get; set; }

		GameObject AdsController;
		public Player playerTemp;

		void Awake()
		{
            Gley.MobileAds.API.Initialize();
			//init the game
			Instance = this;
			menuManager = FindObjectOfType<MainMenu>();
			//spawn the holder to hold the rocket
			RocketHolder = new GameObject();
			RocketHolder.name = "Rocket Hoder";
			//spawn the holder to hold the bullet
			BulletHolder = new GameObject();
			BulletHolder.name = "Bullet Holder";
			//get the player
			
			SoundManager = FindObjectOfType<SoundManager>();

			Application.targetFrameRate = 60;
		}

		void Start()
		{

            //spawn the level prefab
            SpawnLevelBlock();
            //check and spawn the choosen submarine
            SetNewSubmarine();
			//try to get the ads controller
			AdsController = GameObject.Find("AdsController");

            if (!PlayerPrefs.HasKey(DeathCountKey))
            {
                PlayerPrefs.SetInt(DeathCountKey, 0);
            }

            showbannerads();
        }

        void showbannerads()
        {
            if (PlayerPrefs.GetInt(RemoveAdsKey, 0) == 1)
            {
                Debug.Log("Ads are removed. No ad will be shown.");
                return; // Exit the method if ads are removed
            }
            // Implement your ad logic here
            Debug.Log("Showing Ad...");
            Gley.MobileAds.API.ShowBanner(Gley.MobileAds.BannerPosition.Bottom, Gley.MobileAds.BannerType.Banner);
        }
		void Update()
		{
			//update the distance
			if (State == GameState.Playing)
			{
				distance += Speed * Time.deltaTime;
			}

			if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
			{
				PlayerPrefs.DeleteAll();
				UnityEngine.SceneManagement.SceneManager.LoadScene(0);
			}
		}

		public void Play()
		{
			//init the game state
			State = GameState.Playing;
			Player.Play();
			//try to hide the banner ad
			if (AdsController)
				AdsController.SendMessage("HideAdmobBanner", SendMessageOptions.DontRequireReceiver);
		}

		public void SetNewSubmarine()
		{
			//Get the submarine object
			var Submarine = CharacterHolder.Instance.GetPickedCharacter();

			//get the position
			if (Player != null)
			{
				var pos = new Vector3(Player.transform.position.x, 0, 0);
				Destroy(Player.gameObject);
				var newSubmarine = Instantiate(Submarine, pos, Quaternion.identity) as GameObject;
				playerTemp = newSubmarine.GetComponent<Player>();

			}
			else
			{
				Debug.LogError("THERE ARE NO SUBMARINE ON THE SCENE! PLACE ONE!");
			}
		}

		public void SpawnLevelBlock()
		{
			//Check and spawn the level object
			for (int i = LevelBlock.Length - 1; i >= 0; i--)
			{
				if (distance >= LevelBlock[i].distanceReachLevel)
				{
					//if the game is waiting to start, spawn the dummy level
					if (State == GameState.Menu)
					{
						Instantiate(levelEmpty);
						Speed = 5;
					}
					//if check the test level
					else if (spawnTestLevel && testLevelItem != null)
					{
						Debug.Log("===> Only spawn the test level prefab");
						Instantiate(testLevelItem);
						Speed = testSpeed;
					}
					else
					{
						Instantiate(LevelBlock[i].Levels[Random.Range(0, LevelBlock[i].Levels.Length)]);
						Speed = LevelBlock[i].levelSpeed;
					}

					break;
				}
			}
		}

		public void GameOver()
		{
			//try to show ads banner and interstitial
			if (AdsController)
			{
				AdsController.SendMessage("ShowAdmobBanner", SendMessageOptions.DontRequireReceiver);
				AdsController.SendMessage("ShowNormalAd", SendMessageOptions.DontRequireReceiver);
			}
			//check best
			if (Score > GlobalValue.Best)
				GlobalValue.Best = Score;
            
            playfab.sendLeaderboard(Score);
			//Check mission
			MissionManager.Instance.CheckMissions();
			State = GameState.GameOver;
			MainMenu.Instance.GameOver();
			Speed = 0;

            // Get the current death count
            int deathCount = PlayerPrefs.GetInt(DeathCountKey, 0);

            // Increase death count by 1
            deathCount++;
            PlayerPrefs.SetInt(DeathCountKey, deathCount);
            PlayerPrefs.Save();

            // Check if the player has died 3 times
            if (deathCount >= 3)
            {
                ShowAd(); // Call ad method
                PlayerPrefs.SetInt(DeathCountKey, 0); // Reset the counter
                PlayerPrefs.Save();
            }
            Player.Die();
		}
        void ShowAd()
        {
            if (PlayerPrefs.GetInt(RemoveAdsKey, 0) == 1)
            {
                Debug.Log("Ads are removed. No ad will be shown.");
                return; // Exit the method if ads are removed
            }

            // Implement your ad logic here
            Debug.Log("Showing Ad...");
            Gley.MobileAds.API.ShowInterstitial();
        }

        [System.Serializable]
		public class LevelBlocks
		{
			public int distanceReachLevel = 0;
			public float levelSpeed = 3;
			public GameObject[] Levels;
		}

		public void ShowFloatingText(string text, Vector2 positon, Color color)
		{
			GameObject floatingText = Instantiate(FloatingText) as GameObject;

			floatingText.transform.SetParent(menuManager.transform, false);
			floatingText.transform.position = positon;

			var _FloatingText = floatingText.GetComponent<FloatingText>();
			_FloatingText.SetText(text, color);
		}

	}
}