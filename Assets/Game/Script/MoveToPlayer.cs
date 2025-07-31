using UnityEngine;
using System.Collections;
namespace PhoenixaStudio
{
	public class MoveToPlayer : MonoBehaviour
	{
		//set the moving speed
		public float speed = 10;

		void Update()
		{
			//move the object to the player
			transform.position = Vector2.MoveTowards(transform.position, GameManager.Instance.Player.transform.position, speed * Time.deltaTime);
		}
	}
}