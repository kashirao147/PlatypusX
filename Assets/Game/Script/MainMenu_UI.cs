using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace PhoenixaStudio
{
	public class MainMenu_UI : MonoBehaviour
	{
		//init the UI objects
		public Button ShieldBut;
		public Image ShieldImage;
		public Text score;
		public Text distance;

		void Update()
		{
			//set the shield value for image and button
			ShieldImage.fillAmount = GameManager.Instance.Player.shieldEnegry / 100f;
			ShieldBut.interactable = GameManager.Instance.Player.shieldEnegry == 100;
			//display the score and distance value on the UI
			score.text = "SCORE: " + GameManager.Instance.Score;
			distance.text = (int)GameManager.Instance.distance + "";
		}
	}
}