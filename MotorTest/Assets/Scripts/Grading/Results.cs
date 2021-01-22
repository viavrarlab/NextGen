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

    float time;
    float sec;
    float min;

    public Results()
    {

    }
    public Results(string partName, int PickUpCount, bool isPaused)
    {
        this.partName = partName;
        this.PickUpCount = PickUpCount;
        this.isPaused = isPaused;
    }


    public override bool Equals(object obj) => obj is Results res && Equals(res);
    public bool Equals(Results other) => partName == other.partName && PickUpCount != PickUpCount && isPaused;

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


    public override int GetHashCode()
    {
        int hashCode = 1151481828;
        hashCode = hashCode * -1521134295 + base.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(partName);
        hashCode = hashCode * -1521134295 + PickUpCount.GetHashCode();
        hashCode = hashCode * -1521134295 + isPaused.GetHashCode();
        return hashCode;
    }
}
