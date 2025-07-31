using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace PhoenixaStudio
{
	public class ShopItems_Rocket : MonoBehaviour
	{
		//set the price for the item
		public int price = 5;
		//set the amount of the item
		public int amount = 1;
		//max value allowed
		public int max = 100;
		public Text priceTxt;
		public Text currentTxt;
		public AudioClip soundPurchased;
		public AudioClip soundFail;

		// Use this for initialization
		void Start()
		{
			//display the information
			priceTxt.text = price + "";
			currentTxt.text = GlobalValue.Rocket + "/" + max;
		}

		public void Buy()
		{
			//check the remain coin and the limit value
			if (GlobalValue.Coin >= price && GlobalValue.Rocket < max)
			{
				SoundManager.PlaySfx(soundPurchased);
				GlobalValue.Coin -= price;
				GlobalValue.Rocket += amount;
				currentTxt.text = GlobalValue.Rocket + "/" + max;
			}
			else
				SoundManager.PlaySfx(soundFail);
		}
	}
}