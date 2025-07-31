using UnityEngine;
using System.Collections;
namespace PhoenixaStudio
{
	public class AutoDestroy : MonoBehaviour
	{
		//destroy after this time value
		public float destroyAfterTime = 3f;

		void Awake()
		{
			//trigger the destroy event
			Destroy(gameObject, destroyAfterTime);
		}
	}
}