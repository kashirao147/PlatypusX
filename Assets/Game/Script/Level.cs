using UnityEngine;
using System.Collections;
namespace PhoenixaStudio
{
	public class Level : MonoBehaviour
	{
		//spawn the level at this world postion
		public float spawnPosition = 20;
		//destroy the level if it reach to this position
		public float destroyAtPosition = -20;
		bool isSpawnNew = false;

		void Awake()
		{
			//set the position
			transform.position = new Vector3(spawnPosition, 0, 0);
		}

		void Update()
		{
			//only moving when the game state is playing or waiting menu
			if (GameManager.Instance.State == GameManager.GameState.Playing || GameManager.Instance.State == GameManager.GameState.Menu)
				transform.Translate(GameManager.Instance.Speed * -1 * Time.deltaTime, 0, 0);

			//if the position reach to 0, spawn the next level prefab
			if (!isSpawnNew && transform.position.x <= 0)
			{
				isSpawnNew = true;
				GameManager.Instance.SpawnLevelBlock();
			}

			if (transform.position.x <= destroyAtPosition)
			{
				Destroy(gameObject);
			}
		}

		void OnDrawGizmos()
		{
			Gizmos.color = Color.white;
			Gizmos.DrawWireCube(transform.position, new Vector3(20, 10f, 0));
		}
	}
}