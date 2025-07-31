using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace PhoenixaStudio
{
	public class Coin_UI : MonoBehaviour
	{
		//show the text value
		public Text coin;

		void Update()
		{
			//show the text value
			coin.text = GlobalValue.Coin + "";
		}
	}
}