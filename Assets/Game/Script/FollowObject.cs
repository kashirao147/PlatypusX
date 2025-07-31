using UnityEngine;
namespace PhoenixaStudio
{
	public class FollowObject : MonoBehaviour
	{
		[Tooltip("if this target == null, the target will be Main Camera")]
		public Transform target;
		public bool followX = true;
		public bool followY = true;

		void Start()
		{
			//if can't find the target, use camera instead
			if (target == null)
				target = Camera.main.transform;
		}

		void Update()
		{
			//get the following position
			Vector3 follow = target.position;
			//Move the object by checking the X or Y
			if (followX && followY)
				transform.position = new Vector3(follow.x, follow.y, transform.position.z);
			else if (followX)
				transform.position = new Vector3(follow.x, transform.position.y, transform.position.z);
			else if (followY)
				transform.position = new Vector3(transform.position.x, follow.y, transform.position.z);
		}
	}
}