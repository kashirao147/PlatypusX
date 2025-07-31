using UnityEngine;
using System.Collections;
namespace PhoenixaStudio
{
	public class CharacterHolder : MonoBehaviour
	{
		public static CharacterHolder Instance;
		//List of the players
		public GameObject[] Characters;

		void Awake()
		{
			//init
			Instance = this;
			//check and get the player
			GetPickedCharacter();
		}

		public GameObject GetPickedCharacter()
		{
			//Check and return the choosen player
			foreach (var character in Characters)
			{
				var ID = character.GetInstanceID();
				if (GlobalValue.CharacterPicked(0, false) == ID)
				{
					return character;
				}
			}

			//return default character at 1 position
			return Characters[0];
		}
	}
}