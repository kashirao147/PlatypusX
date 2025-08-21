using UnityEngine;
using System.Collections;
namespace PhoenixaStudio
{
	public class PowerUpMagnet : MonoBehaviour
	{
		void Update()
		{
			//only moving when game state is playing mode
			if (GameManager.Instance.State == GameManager.GameState.Playing)
				transform.Translate(GameManager.Instance.Speed * -1 * Time.deltaTime, 0, 0);
		}

		void OnTriggerEnter2D(Collider2D other)
		{
			//Only contact to the player
			if (other.GetComponent<Player>())
			{
				//Active the magnet with the given time
				other.GetComponent<Player>().UseMagnet();
				SoundManager.PlaySfx(GameManager.Instance.SoundManager.soundPowerUpMagnet);

				GlobalValue.CollectMagnetPowerUp++;
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