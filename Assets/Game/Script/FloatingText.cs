using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace PhoenixaStudio
{
	public class FloatingText : MonoBehaviour
	{
		public Text floatingText;
		//Set the text message and color
		public void SetText(string text, Color color)
		{
			floatingText.color = color;
			floatingText.text = text;
		}
	}
}