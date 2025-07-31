using UnityEngine;
using System.Collections;
namespace PhoenixaStudio
{
	public class RandomSpawn : MonoBehaviour
	{
		//place the items that need to be spawned
		public GameObject[] Items;
		//random time to spawn the item
		public int timeMin = 5;
		public int timeMax = 15;
		//set the zone for spawning
		public Vector2 spawnZone;

		// Use this for initialization
		void Start()
		{
			//spawn the item
			StartCoroutine(SpawnTheShark());
		}

		IEnumerator SpawnTheShark()
		{
			//wait the random time value
			yield return new WaitForSeconds(Random.Range(timeMin, timeMax));
			//only moving if game state is playing
			if (GameManager.Instance.State == GameManager.GameState.Playing)
				Instantiate(Items[Random.Range(0, Items.Length)], new Vector2(Random.Range(transform.position.x - spawnZone.x / 2, transform.position.x + spawnZone.x / 2),
					Random.Range(transform.position.y - spawnZone.y / 2, transform.position.y + spawnZone.y / 2)), Quaternion.identity);

			StartCoroutine(SpawnTheShark());
		}

		void OnDrawGizmos()
		{
			Gizmos.color = new Color(1, 1, 1, 0.1f);
			Gizmos.DrawCube(transform.position, spawnZone);
		}
	}
}