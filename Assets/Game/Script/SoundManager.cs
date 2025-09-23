using UnityEngine;
using System.Collections;
/*
 * This is SoundManager
 * In other script, you just need to call SoundManager.PlaySfx(AudioClip) to play the sound
*/
namespace PhoenixaStudio
{
	public class SoundManager : MonoBehaviour
	{
		public static SoundManager Instance;

		public AudioClip introMusic;

		[Tooltip("Play music clip when start")]
		public AudioClip[] musicsGame;
		[Range(0, 1)]
		public float musicsGameVolume = 0.5f;

		[Tooltip("Place the sound in this to call it in another script by: SoundManager.PlaySfx(soundname);")]
		public AudioClip soundClick;
		public AudioClip soundCollectCoin;
		public AudioClip soundPowerUpGun;
		public AudioClip soundPowerUpMagnet;
		public AudioClip soundPowerUpShield;
		public AudioClip soundExplosion;
		public AudioClip SnowCoin;
		public AudioClip CanonFire;
		public AudioClip jump;

		private AudioSource musicAudio;
		private AudioSource soundFx;

		//set the volume for the music
		public static float MusicVolume
		{
			set { Instance.musicAudio.volume = value; }
			get { return Instance.musicAudio.volume; }
		}
		//set the volume for the sound
		public static float SoundVolume
		{
			set { Instance.soundFx.volume = value; }
			get { return Instance.soundFx.volume; }
		}
		// Use this for initialization
		void Awake()
		{
			//init the audio source
			Instance = this;
			musicAudio = gameObject.AddComponent<AudioSource>();
			musicAudio.loop = true;
			musicAudio.volume = 0.5f;
			soundFx = gameObject.AddComponent<AudioSource>();
		}
		void Start()
		{
			//		//Check auido and sound
			if (!GlobalValue.isMusic)
				musicAudio.volume = 0;
			if (!GlobalValue.isSound)
				soundFx.volume = 0;

			if (musicsGame.Length > 0)
				PlayMusic(musicsGame[Random.Range(0, musicsGame.Length)], musicAudio.volume);

			PlaySfx(introMusic, 0.5f);
		}
		//Play the click sound
		public static void Click()
		{
			PlaySfx(Instance.soundClick);
		}
		//play the clip sound
		public static void PlaySfx(AudioClip clip)
		{
			Instance.PlaySound(clip, Instance.soundFx);
		}
		//play the clip sound with the new volume
		public static void PlaySfx(AudioClip clip, float volume)
		{
			Instance.PlaySound(clip, Instance.soundFx, volume);
		}

		public static void PlayMusic(AudioClip clip)
		{
			Instance.PlaySound(clip, Instance.musicAudio);
		}
		//play the clip as music with the new volume
		public static void PlayMusic(AudioClip clip, float volume)
		{
			Instance.PlaySound(clip, Instance.musicAudio, volume);
		}

		private void PlaySound(AudioClip clip, AudioSource audioOut)
		{
			if (clip == null)
			{
				return;
			}

			if (audioOut == musicAudio)
			{
				audioOut.clip = clip;
				audioOut.Play();
			}
			else
				audioOut.PlayOneShot(clip, SoundVolume);
		}

		private void PlaySound(AudioClip clip, AudioSource audioOut, float volume)
		{
			if (clip == null)
			{
				return;
			}

			if (audioOut == musicAudio)
			{
				audioOut.volume = volume;
				audioOut.clip = clip;
				audioOut.Play();
			}
			else
				audioOut.PlayOneShot(clip, SoundVolume * volume);
		}
	}
}