using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace PhoenixaStudio
{
	public class MainMenu_Controller : MonoBehaviour
	{
		//the private parameters
		bool isMoveUp = false;
		public Text bullet;
		public Text rocket;
		public Text Shield;

		bool isFire = false;

		void Update()
		{
			//move the player up
			//if (isMoveUp)
				//GameManager.Instance.Player.MoveUp();
				//Fire the bullet
				if (isFire)
				{
					Debug.Log("Is Firing ");
					GameManager.Instance.Player.FireBullet();
				}
			if (GameManager.Instance.Player.shieldEnegry >= 100 || !GameManager.Instance.Player.isUsingShield )
			{
				Shield.transform.parent.GetChild(1).gameObject.SetActive(true);
			}
			else
			{
				Shield.transform.parent.GetChild(1).gameObject.SetActive(false);
			}
			//Display the rocket and bullet value
			rocket.text = GlobalValue.Rocket + "";
			bullet.text = GlobalValue.Bullet + "";
			Shield.text = GlobalValue.CollectShieldPowerUp + "";
		}

		public void UseShield()
		{
			GameManager.Instance.Player.UseShield();
		}

		public void FireRocket()
		{
			GameManager.Instance.Player.Fire();
		}

		public void MoveUp()
		{
			if (!GameManager.Instance.Player.isSnowLevel)
				isMoveUp = true;
			else
			{
				GameManager.Instance.Player.JumpSnowLevel();
			}
		}
		public void Jump()
		{
			//GameManager.Instance.Player.JumpSnowLevel();
			//isMoveUp = true;
		}

		public void MoveUpOff()
		{
			isMoveUp = false;
		}

		public void FireGun()
		{
			Debug.Log("Fire");
			isFire = true;
		}
		public void FireGunOff()
		{
			Debug.Log("Fire stop");
			isFire = false;
		}
	}
}