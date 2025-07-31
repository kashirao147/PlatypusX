using UnityEngine;
using System.Collections;
namespace PhoenixaStudio
{
	public class Rocket : MonoBehaviour
	{
		//Set the type of the weapon
		public enum RocketType { Rocket, Bullet }
		public RocketType rocketType;
		//damage to deal the player
		public int damage = 10;
		public bool detectSharkFirst;
		//layer of the target
		public LayerMask targetLayer;
		//moving speed
		public float speed = 1;
		//can track the target or not
		public bool isUseRadar = true;
		//the area for checking
		public float radarRadius = 3;

		public GameObject ExplosionFX;
		public AudioClip soundExplosion;

		bool isDetect;
		Transform target;

		void OnEnable()
		{
			gameObject.transform.SetParent(null);
			isDetect = false;
			transform.eulerAngles = Vector3.zero;
		}

		// Update is called once per frame
		void Update()
		{
			//check if use radar then follow the target
			if (!isDetect && isUseRadar)
			{
				//check the target in the zone
				RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position + new Vector3(radarRadius, 0, 0), radarRadius, Vector2.zero, 0, targetLayer);
				if (hits.Length > 0)
				{
					isDetect = true;
					float clostDistance = 999;
					foreach(var obj in hits)
                    {
						//get the obj distance to the rocket for checking
						var _distance = Vector2.Distance(obj.collider.transform.position, transform.position);
						//if the distance is closer than the previous, take this
						if (_distance < clostDistance)
                        {
							clostDistance = _distance;
							target = obj.collider.gameObject.transform;
						}
                    }
				}
				transform.Translate(speed * Time.deltaTime, 0, 0);
			}
			else if (target)
			{
				transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

				//rotate the rocket look to the target
				Vector3 dir = target.position - transform.position;
				var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
				angle = Mathf.Lerp(transform.eulerAngles.z > 180 ? transform.eulerAngles.z - 360 : transform.eulerAngles.z, angle, 0.1f);
				transform.rotation = Quaternion.AngleAxis(angle < 0 ? angle - 360 : angle, Vector3.forward);
			}
			else
			{
				transform.Translate(speed * Time.deltaTime, 0, 0, Space.Self);
			}
		}

		void OnTriggerEnter2D(Collider2D other)
		{
			//try to find the enemy
			var Enemy = other.GetComponent<Enemy>();

			if (Enemy)
			{
				//Deal damage to the enemy
				Enemy.Hit(damage, true);

				SoundManager.PlaySfx(soundExplosion);
				if (ExplosionFX != null)
					Instantiate(ExplosionFX, other.gameObject.transform.position, Quaternion.identity);
				Hide();
			}
		}

		void OnBecameInvisible()
		{
			Hide();
		}

		public virtual void Hide()
		{
			if (GameManager.Instance.RocketHolder && gameObject.activeInHierarchy)
			{
				gameObject.transform.SetParent(rocketType == RocketType.Rocket ? GameManager.Instance.RocketHolder.transform : GameManager.Instance.BulletHolder.transform);
				gameObject.SetActive(false);
			}
			else
				Destroy(gameObject);
		}

		void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(transform.position + new Vector3(radarRadius, 0, 0), radarRadius);
		}

	}
}