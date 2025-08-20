using UnityEngine;
using System.Collections;
namespace PhoenixaStudio
{
	public class PowerUpGun : MonoBehaviour
	{
		//add the bullet amount
		public int bulletAdded = 5;
		//rocket add
		public int rocketAdded = 1;
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
				//Add bullet
				GlobalValue.Bullet += bulletAdded;
				GlobalValue.Rocket += rocketAdded;
				SoundManager.PlaySfx(GameManager.Instance.SoundManager.soundPowerUpGun);

				GlobalValue.CollectBulletPowerUp++;

				Destroy(gameObject);
			}
		}

		void OnbecameInvisible()
		{
			Destroy(gameObject);
		}
	}
}