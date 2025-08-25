using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace PhoenixaStudio
{
	public class MainMenu : MonoBehaviour
	{
		//Place the UI object
		public static MainMenu Instance;
		public GameObject UI;
		public GameObject Controller;
		public GameObject Gameover;
		public GameObject Gamepause;
		public GameObject Shop;
		public GameObject Level;
		public GameObject FirstTimeLevelSelectWindow;
		public GameObject FirstTimeLevelSelectButton;
		public GameObject Achievements;
		public GameObject FriendAndChallenge;
		public GameObject Mission;
		public GameObject Loading;
		public GameObject Coin;

		[Header("Settings")]
		public GameObject Settings;
		public Image sound;
		public Sprite soundOn;
		public Sprite soundOff;
		public Image music;
		public Sprite musicOn;
		public Sprite musicOff;

		void Awake()
		{
			Instance = this;
		}

		// Use this for initialization
		void Start()
		{
			if (!GlobalValue.getGameRestart())
			{
				FirstTimeLevelSelectButton.SetActive(false);
			}
			//Init the UI objects
			UI.SetActive(false);
			Controller.SetActive(false);
			Gameover.SetActive(false);
			Gamepause.SetActive(false);
			Shop.SetActive(false);
			Settings.SetActive(false);
			Mission.SetActive(false);
			Loading.SetActive(false);
			Coin.SetActive(false);
			//Set the audio/music state
			sound.sprite = GlobalValue.isSound ? soundOn : soundOff;
			music.sprite = GlobalValue.isMusic ? musicOn : musicOff;

			SoundManager.SoundVolume = GlobalValue.isSound ? SoundManager.SoundVolume : 0;
			SoundManager.MusicVolume = GlobalValue.isMusic ? SoundManager.MusicVolume : 0;
		}

		public void Play()
		{
			GlobalValue.setGameRestart(0);
			FirstTimeLevelSelectButton.SetActive(false);
			//Trigger the play event
			GameManager.Instance.Play();
			UI.SetActive(true);
			Controller.SetActive(true);
			Coin.SetActive(true);
			
		}

		//called by GameManager
		public void GameOver()
		{
			UI.SetActive(false);
			StartCoroutine(ShowPanelCo(Mission, 1.5f));
			StartCoroutine(ShowPanelCo(Gameover, 2));
		}

		public void Restart()
		{
			//Play sound and restart the game
			SoundManager.PlaySfx(GameManager.Instance.SoundManager.soundClick);
			if (MissionManager.Instance.isAnyTaskCompleted())
				MissionManager.Instance.GetAllRewarded();
			//Show the loading object
			Loading.SetActive(true);
			//SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
			SceneManager.LoadSceneAsync(GlobalValue.getSelectedLevel());
		}


		public void Home()
		{
			//Goto Home menu
			SoundManager.PlaySfx(GameManager.Instance.SoundManager.soundClick);
			SceneManager.LoadSceneAsync("HomeMenu");
		}

		public void OpenShop()
		{
			SoundManager.PlaySfx(GameManager.Instance.SoundManager.soundClick);
			Shop.SetActive(true);
		}
		public void OpenFirstTimeLevelSelect()
		{
			FirstTimeLevelSelectButton.SetActive(false);
			SoundManager.PlaySfx(GameManager.Instance.SoundManager.soundClick);
			FirstTimeLevelSelectWindow.SetActive(true);
		}
		public void OpenAchievements()
		{
			if (!FindFirstObjectByType<PlayFabManager>().isLogin)
			{
				return;
			}
			GameManager.Instance.OpenAchievements();
			SoundManager.PlaySfx(GameManager.Instance.SoundManager.soundClick);
			
		}
		public void OpenOpenFriendAndChallenges()
		{
			if (!FindFirstObjectByType<PlayFabManager>().isLogin)
			{
				return;
			}
			
			SoundManager.PlaySfx(GameManager.Instance.SoundManager.soundClick);
			FriendAndChallenge.SetActive(true);
		}
		public void OpenLevelSelection()
		{
			SoundManager.PlaySfx(GameManager.Instance.SoundManager.soundClick);
			Level.SetActive(true);
		}

		public void OpenSettings()
		{
			SoundManager.PlaySfx(GameManager.Instance.SoundManager.soundClick);
			Settings.SetActive(true);
		}

		public void Exit()
		{
			//Exit the game
			Application.Quit();
		}

		public void Pause()
		{
			//Check pause the game
			if (Time.timeScale == 1)
			{
				Time.timeScale = 0;
				Gamepause.SetActive(true);
				GameManager.Instance.State = GameManager.GameState.Pause;
			}
			else
			{
				Time.timeScale = 1;
				Gamepause.SetActive(false);
				GameManager.Instance.State = GameManager.GameState.Playing;
			}
			SoundManager.PlaySfx(GameManager.Instance.SoundManager.soundClick);
		}

		IEnumerator ShowPanelCo(GameObject panel, float time)
		{
			yield return new WaitForSeconds(time);
			panel.SetActive(true);
		}

		/// <summary>
		/// Settings
		/// </summary>
		/// 
		public void TurnSound()
		{
			//Save the sound state
			GlobalValue.isSound = !GlobalValue.isSound;
			sound.sprite = GlobalValue.isSound ? soundOn : soundOff;

			SoundManager.SoundVolume = GlobalValue.isSound ? 1 : 0;

			SoundManager.PlaySfx(GameManager.Instance.SoundManager.soundClick);
		}

		public void TurnMusic()
		{
			//Save the music state
			GlobalValue.isMusic = !GlobalValue.isMusic;
			music.sprite = GlobalValue.isMusic ? musicOn : musicOff;

			SoundManager.MusicVolume = GlobalValue.isMusic ? GameManager.Instance.SoundManager.musicsGameVolume : 0;

			SoundManager.PlaySfx(GameManager.Instance.SoundManager.soundClick);
		}

		void OnDisable()
		{
			Time.timeScale = 1;
		}
	}
}