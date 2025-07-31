/// <summary>
/// This object is used to trigger when detect play then set all object active
/// </summary>
using UnityEngine;
using System.Collections;
namespace PhoenixaStudio
{
	public class TriggerEnemy : MonoBehaviour
	{
		//place the object need trigger
		public GameObject[] Objects;

		// Use this for initialization
		void Awake()
		{
			//disable the object at the beginning
			if (Objects.Length == 0)
			{
				gameObject.SetActive(false);
				return;
			}
			foreach (var obj in Objects)
				obj.SetActive(false);
		}

		void OnTriggerEnter2D(Collider2D other)
		{
			//only contact with the player
			if (other.GetComponent<Player>())
			{
				foreach (var obj in Objects)
					obj.SetActive(true);
			}
		}
	}
}