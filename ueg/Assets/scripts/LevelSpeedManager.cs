using UnityEngine;

public class LevelSpeedManager : MonoBehaviour
{
    public static LevelSpeedManager Instance;

    public float currentStepDelay = 1f;     
    //public float speedIncreaseInterval = 30f; 
    //public float speedMultiplier = 0.9f;      

    private float timer;

    private void Awake()
    {
        Instance = this;
    }

    //private void Update()
    //{
    //    timer += Time.deltaTime;

    //    if (timer >= speedIncreaseInterval)
    //    {
    //        timer = 0;
    //        currentStepDelay *= speedMultiplier;

    //        if (currentStepDelay < 0.05f)
    //            currentStepDelay = 0.05f; 

    //        Debug.Log("New speed: " + currentStepDelay);
    //    }
    //}
}
