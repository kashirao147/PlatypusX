using UnityEngine;
using System.Collections;
namespace PhoenixaStudio
{
	public class PowerUpShield : MonoBehaviour
	{
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
				other.GetComponent<Player>().shieldEnegry = 100;
				other.GetComponent<Player>().UseShield();
				//Add number of shield is collected
				GlobalValue.CollectShieldPowerUp++;

				Destroy(gameObject);
			}
		}

		void OnbecameInvisible()
		{
			Destroy(gameObject);
		}
	}
}