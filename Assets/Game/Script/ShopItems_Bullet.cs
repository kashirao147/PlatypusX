using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace PhoenixaStudio
{
	public class ShopItems_Bullet : MonoBehaviour
	{
		//set the price for the item
		public int price = 10;
		//set the amount of the item
		public int amount = 10;
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
			currentTxt.text = GlobalValue.Bullet + "/" + max;
		}

		public void Buy()
		{
			//check the remain coin and the limit value
			if (GlobalValue.Coin >= price && GlobalValue.Bullet < max)
			{
				SoundManager.PlaySfx(soundPurchased);
				GlobalValue.Coin -= price;
				GlobalValue.Bullet += amount;
				currentTxt.text = GlobalValue.Bullet + "/" + max;
			}
			else
				SoundManager.PlaySfx(soundFail);
		}
	}
}