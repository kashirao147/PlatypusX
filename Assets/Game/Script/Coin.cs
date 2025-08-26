using UnityEngine;
using System.Collections;
namespace PhoenixaStudio
{
	public class Coin : MonoBehaviour
	{
		public bool isSnowCoin = false;
		//the effect that will be shown when collected the coin
		public GameObject CollectedEffect;
		public GameObject SparkEffect;

		//called by Submarine script
		public void Collect()
		{
			if (CollectedEffect != null)
				Instantiate(CollectedEffect, transform.position, Quaternion.identity);
			if (SparkEffect != null)
				Instantiate(SparkEffect, transform.position, Quaternion.identity);
			//play sound
			if (isSnowCoin)
			{
			SoundManager.PlaySfx(GameManager.Instance.SoundManager.SnowCoin);	
			}
			else
			SoundManager.PlaySfx(GameManager.Instance.SoundManager.soundCollectCoin);
			//destroy the coin after collected
			Destroy(gameObject);
		}
	}
}