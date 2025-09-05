using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
namespace PhoenixaStudio
{
	public class HomeMenu : MonoBehaviour
	{
		//auto play game after this time value
		public float gotoPlayIn = 1;
		float time;

		void Update()
		{
			//calculating the time
			time += Time.deltaTime;
			if (time > gotoPlayIn)
			{
				GotoPlay();
				enabled = false;
			}
		}

		public void GotoPlay()
		{
			GlobalValue.UnlockLevel("Level1");
			GlobalValue.setGameRestart(1);
			GlobalValue.setCanShowNamePannel(1);
			SceneManager.LoadSceneAsync(GlobalValue.getSelectedLevel());
			
		}
	}
}