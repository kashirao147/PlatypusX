using UnityEngine;
using System.Collections;
namespace PhoenixaStudio
{
	public class BackGroundController : MonoBehaviour
	{
		//set the speed for background. midground. forceground
		public Renderer Background;
		//Set moving speed for the background
		public float speedBG = 0.01f;
		public Renderer Midground;
		//Set moving speed for the Midground
		public float speedMG = 0.02f;
		public Renderer Forceground;
		//Set moving speed for the Forceground
		public float speedFG = 0.03f;
		public Renderer Fish;
		public float speedFish = 0.015f;
		float x;

		void Update()
		{
			if (GameManager.Instance.State == GameManager.GameState.Playing || GameManager.Instance.State == GameManager.GameState.Menu)
			{
				//caculating the x value of the target and the original position
				x += GameManager.Instance.Speed * Time.deltaTime;

				if (Background != null)
				{
					//caculating the offset value then set to the texture offset value
					var offset = (x * speedBG) % 1;
					Background.material.mainTextureOffset = new Vector2(offset, Background.material.mainTextureOffset.y);
				}
				if (Midground != null)
				{
					//caculating the offset value then set to the texture offset value
					var offset = (x * speedMG) % 1;
					Midground.material.mainTextureOffset = new Vector2(offset, Midground.material.mainTextureOffset.y);
				}
				if (Forceground != null)
				{
					//caculating the offset value then set to the texture offset value
					var offset = (x * speedFG) % 1;
					Forceground.material.mainTextureOffset = new Vector2(offset, Forceground.material.mainTextureOffset.y);
				}
				if (Fish != null)
				{
					//caculating the offset value then set to the texture offset value
					var offset = (x * speedFish) % 1;
					Fish.material.mainTextureOffset = new Vector2(offset, Fish.material.mainTextureOffset.y);
				}
			}
		}
	}
}