using UnityEngine;
using System.Collections;
namespace PhoenixaStudio
{
	public class Magnet : MonoBehaviour
	{
		//time of the magnet
		float timer;
		float timeCounter = 0;
		
		public void init(float time)
		{
			//init the time value
			timer = time;
			timeCounter = 0;
		}

		// Update is called once per frame
		void Update()
		{
			//calculating the time remain
			timeCounter += Time.deltaTime;
			if (timeCounter >= timer)
				gameObject.SetActive(false);
		}

		void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Coin"))
			{
				other.gameObject.AddComponent<MoveToPlayer>();
			}
		}
	}
}