using UnityEngine;

public class RandomSpriteOnStart : MonoBehaviour
{
    [SerializeField] private SpriteRenderer target;   // optional; auto-fills if left empty
    [SerializeField] private Sprite[] options;        // assign your sprites here

    private void Start()
    {
        if (target == null) target = GetComponent<SpriteRenderer>();
        if (options != null && options.Length > 0)
        {
            target.sprite = options[Random.Range(0, options.Length)];
        }
    }
}
 