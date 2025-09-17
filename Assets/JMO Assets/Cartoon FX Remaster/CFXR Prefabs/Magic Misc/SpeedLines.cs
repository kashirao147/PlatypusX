using UnityEngine;

public class SpeedLines : MonoBehaviour
{
    public GameObject linePrefab;  // thin white line sprite
    public int lineCount = 20;
    public float speed = 5f;
    public float lineSpacing = 1f;

    void Start()
    {
        for (int i = 0; i < lineCount; i++)
        {
            Vector3 pos = new Vector3( i * lineSpacing,Random.Range(-5f, 5f), 0);
            Instantiate(linePrefab, pos, Quaternion.identity, transform);
        }
    }
 
    void Update()
    {
        foreach (Transform line in transform)
        {
            line.Translate(Vector3.left * speed * Time.deltaTime);

            if (line.position.x < -9f) // off-screen
            {
                line.position = new Vector3( 9f,Random.Range(-5f, 5f), 0);
            }
        }
    }
}
