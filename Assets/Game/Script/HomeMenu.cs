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
			SceneManager.LoadSceneAsync("Snow Level");
		}
	}
}