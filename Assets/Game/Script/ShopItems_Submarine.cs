using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace PhoenixaStudio
{
	public class ShopItems_Submarine : MonoBehaviour
	{
		//set the price for the submarine
		public int price;
		//place the submarine prefab
		public GameObject CharacterPrefab;
		//make this submarine is free to use
		public bool unlockDefault = false;
		public GameObject UnlockButton;
		public GameObject StateButton;

		public Text pricetxt;
		public Text state;

		public AudioClip soundPurchased;
		public AudioClip soundFail;

		bool isUnlock;
		SoundManager soundManager;
		int CharacterID;

		// Use this for initialization
		void Start()
		{
			//get the ID of the prefab
			CharacterID = CharacterPrefab.GetInstanceID();
			soundManager = FindObjectOfType<SoundManager>();
			//check if it is free to use
			if (unlockDefault)
				isUnlock = true;
			else
				isUnlock = GlobalValue.isCharacterUnlocked(CharacterID);
			UnlockButton.SetActive(!isUnlock);
			StateButton.SetActive(isUnlock);
			pricetxt.text = price.ToString();
		}

		void Update()
		{
			//if it is not unlocked, no display the status information
			if (!isUnlock)
				return;

			if (GlobalValue.CharacterPicked(0, false) == CharacterID || (GlobalValue.CharacterPicked(0, false) == 0 && unlockDefault))
				state.text = "Picked";
			else
				state.text = "Choose";
		}

		public void Unlock()
		{
			SoundManager.PlaySfx(soundManager.soundClick);

			if (GlobalValue.Coin >= price)
			{
				GlobalValue.Coin -= price;
				//Unlock the submarine
				GlobalValue.UnlockCharacter(CharacterID);
				isUnlock = true;
				//			Locked.SetActive (false);
				UnlockButton.SetActive(!isUnlock);
				StateButton.SetActive(isUnlock);

				SoundManager.PlaySfx(soundPurchased);
			}
			else
				SoundManager.PlaySfx(soundFail);
		}

		public void Pick()
		{
			SoundManager.PlaySfx(soundManager.soundClick);
			GlobalValue.CharacterPicked(CharacterID, true);
		}
	}
}