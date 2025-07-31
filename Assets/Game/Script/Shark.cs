using UnityEngine;
using System.Collections;
namespace PhoenixaStudio
{
	public class Shark : Enemy
	{
		//set the health of the shark
		public int health = 50;
		//set the random min/max of the speed
		public float speedMin = 2;
		public float speedMax = 4;
		float speed;
		[Range(0, 20)]
		public float randomScale = 10f;
		Animator anim;
		bool isDead = false;
		void Start()
		{
			//get the animator
			anim = GetComponent<Animator>();
			//get the random scale of the shark
			var randomscale = Random.Range(-randomScale, randomScale);
			transform.localScale = new Vector3(transform.localScale.x + randomscale * 0.01f, transform.localScale.y + randomscale * 0.01f, transform.localScale.z);
			speed = Random.Range(speedMin, speedMax) + GameManager.Instance.Speed;
		}

		void Update()
		{
			//only move if the game state is playing
			if (GameManager.Instance.State == GameManager.GameState.Playing)
				transform.Translate(speed * Time.deltaTime * -1, 0, 0);
		}

		public override void Hit(int damage, bool forceDestroy)
		{
			//deduce the health
			health -= damage;
			if (health <= 0)
				Die();
			else
			{
				if(anim != null)
				anim.SetTrigger("Hurt");
			}
		}

		void Die()
		{
            //set the dead trigger
            if (anim != null)
                anim.SetTrigger("Die");
			isDead = true;
			//add the score
			GameManager.Instance.Score += score;
			GameManager.Instance.ShowFloatingText(score + "", transform.position, Color.yellow);

			GlobalValue.SharkKilled++;
			if(GetComponent<BoxCollider>()!=null)
				GetComponent<BoxCollider2D>().enabled = false;
		}

		//call by animation event Die
		public void Destroy()
		{
			Destroy(gameObject);
		}

		void OnTriggerEnter2D(Collider2D other)
		{
			if (isDead)
				return;

			var Player = other.GetComponent<Player>();
			if (Player)
			{
                if (anim != null)
                    anim.SetTrigger("Attack");
				Player.Damage(damage);
			}
		}
	}
}