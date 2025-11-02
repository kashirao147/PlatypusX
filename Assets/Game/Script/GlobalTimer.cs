using UnityEngine;
using System;
using Unity.VisualScripting;

public class GlobalTimer : MonoBehaviour
{
    public enum TimerType { Scaled, Unscaled }

    private static GlobalTimer _instance;

    private float currentTime;
    public bool isRunning;
    private TimerType timerType = TimerType.Scaled;

    // Events (optional)
    public static event Action<float> OnTimerUpdated;
    public static event Action OnTimerFinished;

    // --- UNITY LIFECYCLE ---

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static bool isTimerRunning()
    {
        return _instance.isRunning;
    }


    private void Update()
    {
        if (!isRunning)
            return;

        // Pick correct deltaTime
        float delta = (timerType == TimerType.Scaled) ? Time.deltaTime : Time.unscaledDeltaTime;

        currentTime -= delta;
        Debug.Log(currentTime + "          Time");
        OnTimerUpdated?.Invoke(currentTime);

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isRunning = false;
            OnTimerFinished?.Invoke();
        }
    }

    // --- INTERNAL HELPER ---

    private static void EnsureInstance()
    {
        if (_instance == null)
        {
            GameObject go = new GameObject("GlobalTimer");
            _instance = go.AddComponent<GlobalTimer>();
            DontDestroyOnLoad(go);
        }
    }

    // --- STATIC API ---

    public static void StartTimer(TimerType type = TimerType.Scaled)
    {
        EnsureInstance();
        _instance.timerType = type;
        _instance.isRunning = true;
    }

    public static void PauseTimer()
    {
        EnsureInstance();
        _instance.isRunning = false;
    }

    public static void ResetTimer()
    {
        EnsureInstance();
        _instance.currentTime = 0f;
        _instance.isRunning = false;
    }

    public static void SetTime(float seconds)
    {
        EnsureInstance();
        _instance.currentTime = Mathf.Max(seconds, 0f);
    }

    public static float GetTime()
    {
        EnsureInstance();
        return _instance.currentTime;
    }

    public static bool IsRunning()
    {
        EnsureInstance();
        return _instance.isRunning;
    }

    public static TimerType GetTimerType()
    {
        EnsureInstance();
        return _instance.timerType;
    }
}
