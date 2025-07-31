using UnityEngine;
using System.Collections;
namespace PhoenixaStudio
{
	public class Coin : MonoBehaviour
	{
		//the effect that will be shown when collected the coin
		public GameObject CollectedEffect;

		//called by Submarine script
		public void Collect()
		{
			if (CollectedEffect != null)
				Instantiate(CollectedEffect, transform.position, Quaternion.identity);
			//play sound
			SoundManager.PlaySfx(GameManager.Instance.SoundManager.soundCollectCoin);
			//destroy the coin after collected
			Destroy(gameObject);
		}
	}
}