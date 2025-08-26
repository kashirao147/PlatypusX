using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WordParticlesScene : MonoBehaviour
{
    [Header("Drop your already-placed particle GameObjects here (scene objects)")]
    public List<GameObject> particleObjects = new List<GameObject>();

    [Header("Options")]
    public bool forceWorldSpace = true;        // keep particles static in world
    public bool clearBeforePlay = true;        // Clear() before Play()
    public bool onlyPickIfStopped = true;      // prefer an object that's idle
    public float cleanupPadding = 0.75f;       // extra time for sub-emitters

    private static WordParticlesScene _inst;

    void Awake()
    {
        _inst = this;

        // Prep the placed particles so they don't auto-play.
        foreach (var go in particleObjects.Where(p => p != null))
        {
            var systems = go.GetComponentsInChildren<ParticleSystem>(true);
            foreach (var ps in systems)
            {
                var main = ps.main;
                if (forceWorldSpace) main.simulationSpace = ParticleSystemSimulationSpace.World;
                main.playOnAwake = false;
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                if (clearBeforePlay) ps.Clear(true);
            }
        }
    }

    // ===================== STATIC API (easy calls) ==========================

    /// <summary>Play ONE random pre-placed effect at its own position.</summary>
    public static void PlayRandom()
    {
        if (_inst == null || _inst.particleObjects.Count == 0) return;
        _inst.PlayRandomInternal();
    }

    /// <summary>Play a specific entry by index (0..n-1) at its own position.</summary>
    public static void PlayIndex(int index)
    {
        if (_inst == null) return;
        _inst.PlayObject(Mathf.Clamp(index, 0, _inst.particleObjects.Count - 1));
    }

    /// <summary>Play the first entry whose name contains given text (e.g., "BOOM").</summary>
    public static void PlayByNameContains(string text)
    {
        if (_inst == null || string.IsNullOrEmpty(text)) return;
        int idx = _inst.particleObjects.FindIndex(p => p && p.name.ToLower().Contains(text.ToLower()));
        if (idx >= 0) _inst.PlayObject(idx);
    }

    /// <summary>
    /// NEW: Spawn a short-lived clone of a random particle and play it at a world position.
    /// Keeps it static in world-space and auto-destroys after it finishes.
    /// </summary>
    public static void PlayRandomAt(Vector3 worldPosition)
    {
        if (_inst == null || _inst.particleObjects.Count == 0) return;
        _inst.SpawnCloneAt(worldPosition, _inst.ChooseAnyIndex());
    }

    /// <summary>NEW: Same as above, but choose which effect by index.</summary>
    public static void PlayIndexAt(int index, Vector3 worldPosition)
    {
        if (_inst == null || _inst.particleObjects.Count == 0) return;
        index = Mathf.Clamp(index, 0, _inst.particleObjects.Count - 1);
        _inst.SpawnCloneAt(worldPosition, index);
    }

    // ======================== Implementation ================================

    void PlayRandomInternal()
    {
        int chosen;

        if (onlyPickIfStopped)
        {
            var free = new List<int>();
            for (int i = 0; i < particleObjects.Count; i++)
                if (particleObjects[i] && AllStopped(particleObjects[i])) free.Add(i);

            chosen = free.Count > 0 ? free[Random.Range(0, free.Count)] : ChooseAnyIndex();
        }
        else chosen = ChooseAnyIndex();

        PlayObject(chosen);
    }

    int ChooseAnyIndex()
    {
        return Random.Range(0, particleObjects.Count);
    }

    bool AllStopped(GameObject go)
    {
        var systems = go.GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in systems) if (ps.isPlaying) return false;
        return true;
    }

    void PlayObject(int idx)
    {
        var go = particleObjects[idx];
        if (!go) return;

        var systems = go.GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in systems)
        {
            if (clearBeforePlay) ps.Clear(true);
            ps.Play(true);
        }
    }

    // ---- clone-and-play at position ----------------------------------------

    void SpawnCloneAt(Vector3 position, int sourceIndex)
    {
        var src = particleObjects[sourceIndex];
        if (!src) return;

        // clone as a top-level world object
        var clone = Instantiate(src, position, src.transform.rotation, null);

        float maxLife = 0f;
        foreach (var ps in clone.GetComponentsInChildren<ParticleSystem>(true))
        {
            var main = ps.main;
            if (forceWorldSpace) main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.playOnAwake = false;

            if (clearBeforePlay) ps.Clear(true);
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Play(true);

            // rough lifetime estimate (duration + lifetime)
            float life = main.duration;
            if (main.startLifetime.mode == ParticleSystemCurveMode.TwoConstants)
                life += main.startLifetime.constantMax;
            else
                life += main.startLifetime.constant;

            if (life > maxLife) maxLife = life;
        }

        Destroy(clone, maxLife + cleanupPadding);
    }
}
