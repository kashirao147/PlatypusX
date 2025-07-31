using UnityEngine;
using System.Collections;
namespace PhoenixaStudio
{
	public class AutoRotate : MonoBehaviour
	{
		//set the speed for the rotation
		public float speed = 100;

		void Update()
		{
			//rotate the object around the z axis
			transform.RotateAround(transform.position, Vector3.forward, speed * Time.deltaTime);
		}
	}
}