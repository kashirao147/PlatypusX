using UnityEngine;
using UnityEngine.UI;

namespace PhoenixaStudio
{
    public class ShopPowerupItem : MonoBehaviour
    {
        public enum SourceType { Magnet,Speedboost, shield }

        [Header("What are we selling?")]
        public SourceType source = SourceType.Magnet;

        

        [Header("Pricing")]
        public int price = 10;          // coin cost per purchase
        public int amount = 1;          // how many items added per purchase
        public int max = 99;            // hard cap

        [Header("UI")]
        public Text priceTxt;
        public Text currentTxt;

        [Header("Audio")]
        public AudioClip soundPurchased;
        public AudioClip soundFail;

        void Start()
        {
            priceTxt.text = price + "";
			currentTxt.text = PowerUpValue(source) + "/" + max;
            
        }

         public float PowerUpValue(SourceType type)
        {
                    switch (type)
                    {
                     
                        
                            

                        case SourceType.shield:
                        return GlobalValue.CollectShieldPowerUp;
                            

                        case SourceType.Magnet:
                        return GlobalValue.CollectMagnetPowerUp;
                            

                        
                            
                        case SourceType.Speedboost:
                        return GlobalValue.CollectSpeedBoostPowerUp;
                            

                        
                            
                            
                        default:
                            return 0f;
                    }

        }

        public void Buy()
		{
			//check the remain coin and the limit value
			if (GlobalValue.Coin >= price && PowerUpValue(source) < max)
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
