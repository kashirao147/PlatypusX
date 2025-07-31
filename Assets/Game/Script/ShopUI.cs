using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PhoenixaStudio
{
    public class ShopUI : MonoBehaviour
    {
        public GameObject panelItem, panelIAP;

        private void OnEnable()
        {
            panelIAP.SetActive(false);
            panelItem.SetActive(true);
        }

        public void SwitchPanel(GameObject obj)
        {
            SoundManager.Click();
            panelIAP.SetActive(panelIAP == obj);
            panelItem.SetActive(panelItem == obj);
        }
    }
}