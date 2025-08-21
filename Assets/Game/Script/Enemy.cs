using System.Collections;
using UnityEngine;
namespace PhoenixaStudio
{
	public class Enemy : MonoBehaviour
	{
		public GameObject CollectedEffect;
		//deal damage to the player
		public int damage = 20;
		//add the score when this enemy get killed
		public int score = 10;
		//destroy on hit or not
		public bool destroyWhenHit = false;
		//destroy effect
		public GameObject destroyFX;
		public AudioClip soundDestroy;

		[Header("Enmey reference")]
		public bool isHumanEnemy;
		[SerializeField] Animator animator;

		//called by Player
		public virtual void Hit(int takedamage, bool forceDestroy = false)
		{
			if (isHumanEnemy)
			{

				AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

				// Example: check if "Run" animation is playing
				if (stateInfo.IsName("Die"))
				{
					return;
				}
				animator.Play("Attacking");
			}
			//check if this object is the bomb
			if (gameObject.CompareTag("Bomb"))
			{
				SoundManager.PlaySfx(GameManager.Instance.SoundManager.soundExplosion);

			}
			//check and spawn the destroy fx
			if (destroyFX)
				Instantiate(destroyFX, transform.position, Quaternion.identity);
			//play destroy sound
			SoundManager.PlaySfx(soundDestroy);
			//add the score
			GameManager.Instance.Score += score;
			GameManager.Instance.ShowFloatingText(score + "", transform.position, Color.yellow);

			if (gameObject.CompareTag("Bomb"))
				GlobalValue.BombDestroy++;

			if (forceDestroy || destroyWhenHit)
			{

				if (isHumanEnemy)
				{


					StartCoroutine(CoinSend());
					FindAnyObjectByType<ShakeCamera>().DoShake();
					
					animator.Play("Die");
				}
				else
				{

					Destroy(gameObject);
				}
			}
			else

				GetComponent<Collider2D>().enabled = false;
		}
		public void DestroyObject()
		{
			Destroy(gameObject);
		}

		IEnumerator CoinSend()
		{
			for (int i = 0; i < 5; i++)
			{
				if(CollectedEffect!=null)
				Instantiate(CollectedEffect, transform.position, Quaternion.identity);
				yield return new WaitForSeconds(0.02f);
				GlobalValue.Coin++;
				
			}
		}
	}
	
	
}