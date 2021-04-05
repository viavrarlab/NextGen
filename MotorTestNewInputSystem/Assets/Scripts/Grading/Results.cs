using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Results
{
    public string partName;
    public int PickUpCount;
    public bool isPaused;
    public string PartPickTime;
    public int CorrectPlacementOrder;
    public int ActualPlacementID;


    float time;
    float sec;
    float min;

    public Results()
    {

    }

    public void StartTimer(MonoBehaviour mono)
    {
        mono.StartCoroutine(PartTimer());
    }
    public void StopTimer(MonoBehaviour mono)
    {
        mono.StopCoroutine(PartTimer());
    }

    public IEnumerator PartTimer()
    {
        while (!isPaused)
        {
            time += Time.deltaTime;
            sec = (int)(time % 60);
            min = (int)(time / 60);

            PartPickTime = "Time elapsed - " + string.Format("{0:00}:{1:00}", min, sec);
            yield return PartPickTime;
        }
        yield return null;
    }

}
