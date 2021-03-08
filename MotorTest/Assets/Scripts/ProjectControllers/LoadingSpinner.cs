using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingSpinner : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        
        this.gameObject.transform.Rotate(new Vector3(0,0,20f)* Time.deltaTime,Space.Self);
    }
}
