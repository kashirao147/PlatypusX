using UnityEngine;
using UnityEngine.UI;
namespace PhoenixaStudio
{
    public class ShopItemUI : MonoBehaviour
    {
        public enum ITEM_TYPE { iap1, iap2, iap3, removeAds }
        //choose the type of the IAP item
        public ITEM_TYPE itemType;
        //rewarded for purchasing
        public int rewarded = 100;
        public float price = 100;
        public AudioClip soundRewarded;

        public Text priceTxt, rewardedTxt;

        //update the status for coins, value
        private void Update()
        {
            UpdateStatus();
        }

        void UpdateStatus()
        {
            if (itemType == ITEM_TYPE.removeAds)
            {
                //show the remove ads price
                //priceTxt.text = "$" + price;
                priceTxt.text = Purchaser.Instance.iapRemoveAdText;
                //check if already purchasing it
                if (GlobalValue.RemoveAds)
                    gameObject.SetActive(false);
            }
            else
            {
                //priceTxt.text = "$" + price;
                switch (itemType)
                {
                    case ITEM_TYPE.iap1:
                        priceTxt.text = Purchaser.Instance.iapPriceText1;
                        break;
                    case ITEM_TYPE.iap2:
                        priceTxt.text = Purchaser.Instance.iapPriceText2;
                        break;
                    case ITEM_TYPE.iap3:
                        priceTxt.text = Purchaser.Instance.iapPriceText3;
                        break;
                }
                rewardedTxt.text = "+" + rewarded;
            }
        }

        public void Buy()
        {
            switch (itemType)
            {
                //check the tyoe and do the action to buy IAP
                case ITEM_TYPE.iap1:
                    Purchaser.iAPResult += Purchaser_iAPResult;
                    Purchaser.Instance.BuyItem1();
                    break;
                //check the tyoe and do the action to buy IAP
                case ITEM_TYPE.iap2:
                    Purchaser.iAPResult += Purchaser_iAPResult;
                    Purchaser.Instance.BuyItem2();
                    break;
                //check the tyoe and do the action to buy IAP
                case ITEM_TYPE.iap3:
                    Purchaser.iAPResult += Purchaser_iAPResult;
                    Purchaser.Instance.BuyItem3();
                    break;
                case ITEM_TYPE.removeAds:
                    Purchaser.iAPResult += Purchaser_iAPResult;
                    Purchaser.Instance.BuyRemoveAds();
                    break;
            }
        }

        private void Purchaser_iAPResult(int id)
        {
            //check the tyoe and do the action
            if (itemType == ITEM_TYPE.removeAds)
            {
                GlobalValue.RemoveAds = true;
            }
            else
            {
                Purchaser.iAPResult -= Purchaser_iAPResult;
                GlobalValue.Coin += rewarded;
                SoundManager.PlaySfx(soundRewarded);
                UpdateStatus();
            }
        }
    }
}