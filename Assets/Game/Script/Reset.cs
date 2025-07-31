using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
namespace PhoenixaStudio
{
	public class Reset : MonoBehaviour
	{
		//Reset all data
		public void ResetAll()
		{
			//Delete all saved data
			PlayerPrefs.DeleteAll();
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}
}