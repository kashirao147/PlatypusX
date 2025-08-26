using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomParticleBurst : MonoBehaviour
{
    [Header("Scene particle objects (pre-placed). Drag your LavaFloating objects here.")]
    [SerializeField] private List<GameObject> particleObjects = new List<GameObject>();

    [Header("Burst settings")]
    [Min(0.05f)] [SerializeField] private float intervalSeconds = 3f; // time between bursts
    [SerializeField] private int countPerBurst = 2;                   // how many to play each burst
  

    private Coroutine loop;
    void Start()
    {
        TriggerBurst();
    }




    /// <summary>Manually trigger one burst (plays 'countPerBurst' random effects).</summary>
    [ContextMenu("Trigger Burst Now")]
    public void TriggerBurst() => StartCoroutine(BurstLoop());

    private IEnumerator BurstLoop()
    {
        while (true)
        {
            PlayRandomBurst();

            yield return new WaitForSeconds(intervalSeconds);
        }
    }

    private void PlayRandomBurst()
    {
        if (particleObjects == null || particleObjects.Count == 0) return;


        Debug.Log("Brust");
       
        // Shuffle and take up to countPerBurst unique indices
       
        int toPlay = Mathf.Clamp(countPerBurst, 1, particleObjects.Count);
        for (int k = 0; k < toPlay; k++)
            particleObjects[Random.Range(0, particleObjects.Count)].GetComponent<ParticleSystem>().Play();
    }

   

   
  
}
