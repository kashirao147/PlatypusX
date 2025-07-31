using UnityEngine;
using UnityEngine.UI;
namespace PhoenixaStudio
{
	public class MainMenu_GameOver : MonoBehaviour
	{
		//init the text object
		public Text score;
		public Text best;

		void Start()
		{
			//display the score and best value
			score.text = GameManager.Instance.Score + "";
			best.text = "Best: " + GlobalValue.Best;
		}
	}
}