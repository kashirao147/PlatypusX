using UnityEngine; 
using System.Collections;
namespace PhoenixaStudio
{
	public class ShakeCamera : MonoBehaviour
	{
		//setup the size of shaking
		public bool shakePosition;
		public bool shakeRotation;
		//set the intensity for the shaking
		public float shakeIntensity = 0.5f;
		public float shakeDecay = 0.02f;

		private Vector3 OriginalPos;
		private Quaternion OriginalRot;
		private bool isShakeRunning = false;

		public void DoShake()
		{
			//init the original position and rotation
			OriginalPos = transform.position;
			OriginalRot = transform.rotation;

			StartCoroutine("ProcessShake");
		}

		IEnumerator ProcessShake()
		{
			//check and make the shaking
			if (!isShakeRunning)
			{
				isShakeRunning = true;
				float currentShakeIntensity = shakeIntensity;
				//check the remain time
				while (currentShakeIntensity > 0)
				{
					if (shakePosition)
					{
						transform.position = OriginalPos + Random.insideUnitSphere * currentShakeIntensity;
					}
					if (shakeRotation)
					{
						transform.rotation = new Quaternion(OriginalRot.x + Random.Range(-currentShakeIntensity, currentShakeIntensity) * .2f,
							OriginalRot.y + Random.Range(-currentShakeIntensity, currentShakeIntensity) * .2f,
							OriginalRot.z + Random.Range(-currentShakeIntensity, currentShakeIntensity) * .2f,
							OriginalRot.w + Random.Range(-currentShakeIntensity, currentShakeIntensity) * .2f);
					}
					currentShakeIntensity -= shakeDecay;
					yield return null;
				}

				isShakeRunning = false;
			}
		}
	}
}