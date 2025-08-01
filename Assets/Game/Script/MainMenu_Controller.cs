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

		bool isFire = false;

		void Update()
		{
			//move the player up
			if (isMoveUp)
				GameManager.Instance.Player.MoveUp();
			//Fire the bullet
			if (isFire)
				GameManager.Instance.Player.FireBullet();
			//Display the rocket and bullet value
			rocket.text = GlobalValue.Rocket + "";
			bullet.text = GlobalValue.Bullet + "";
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
			isFire = true;
		}
		public void FireGunOff()
		{
			isFire = false;
		}
	}
}