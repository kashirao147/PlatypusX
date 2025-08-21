using UnityEngine;
using System.Collections;
namespace PhoenixaStudio
{
	public class PowerUpSpeedBoost : MonoBehaviour
	{
		public GameObject CollectingEffect;
		void Update()
		{
			//only moving if game state is playing
			if (GameManager.Instance.State == GameManager.GameState.Playing)
				transform.Translate(GameManager.Instance.Speed * -1 * Time.deltaTime, 0, 0);
		}

		void OnTriggerEnter2D(Collider2D other)
		{
			//Only contact to the player
			if (other.GetComponent<Player>())
			{
				if (CollectingEffect != null)
					Instantiate(CollectingEffect, transform.position, Quaternion.identity);
				other.GetComponent<Player>().UseSpeedBoost();
				//Add number of speed boost powerups collected
				GlobalValue.CollectSpeedBoostPowerUp++;
				GlobalValue.RefreashIngameMissionUI();

				Destroy(gameObject);
			}
		}

		void OnbecameInvisible()
		{
			Destroy(gameObject);
		}
	}
} 