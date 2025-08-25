using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace PhoenixaStudio
{
	public class ShopItem_Level : MonoBehaviour
	{
		//set the price for the submarine
		public int price;
		
		//make this submarine is free to use
		public bool unlockDefault = false;
		public GameObject UnlockButton;
        public string LevelName;
		public Text pricetxt;
		public AudioClip soundPurchased;
		public AudioClip soundFail;

		bool isUnlock;
		SoundManager soundManager;
		

		// Use this for initialization
		void Start()
		{
			//get the ID of the prefab
			
			soundManager = FindObjectOfType<SoundManager>();
			//check if it is free to use
			if (unlockDefault)
				isUnlock = true;
			else
				isUnlock = GlobalValue.isUnlockLevel(LevelName);
			//UnlockButton.SetActive(!isUnlock);
            if (isUnlock)
            {
                pricetxt.text = "Purchased";
                pricetxt.transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {

                pricetxt.text = price.ToString();
            }
		}

		void Update()
		{
			//if it is not unlocked, no display the status information
			if (!isUnlock)
				return;

			
		}

		public void Unlock()
		{
            
			SoundManager.PlaySfx(soundManager.soundClick);

            if (GlobalValue.Coin >= price)
            {
                MessagePopup.ShowPopup("Purchased " + LevelName);
                GlobalValue.Coin -= price;
                //Unlock the submarine
                GlobalValue.UnlockLevel(LevelName);
                isUnlock = true;
                 pricetxt.text = "Purchased";
                pricetxt.transform.GetChild(0).gameObject.SetActive(false);

                //			Locked.SetActive (false);
                //UnlockButton.SetActive(!isUnlock);


                SoundManager.PlaySfx(soundPurchased);
            }
            else
            {
                MessagePopup.ShowPopup("Failed to Purchased " + LevelName);
				SoundManager.PlaySfx(soundFail);
            }
		}

		
	}
}