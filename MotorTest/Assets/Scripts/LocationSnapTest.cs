using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationSnapTest : MonoBehaviour
{
    float snapSpeed = 2f;
    float t = 0f;
    
    float StartX;
    float EndX;
    bool isWithInX;
    float StartY;
    float EndY;
    bool isWithInY;
    float StartZ;
    float EndZ;
    bool isWithInZ;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        checkLocationTest();
    }
    void checkLocationTest()
    {
        print(gameObject.transform.position.ToString());
        if(.5f < gameObject.transform.position.z && gameObject.transform.position.z < 1f)
        {
            t += Time.deltaTime / snapSpeed;
            Vector3 CurrentPosition = Vector3.Lerp(gameObject.transform.position, new Vector3(0f, 0f, .7f), t);
            gameObject.transform.position = CurrentPosition;
            print("atrodas starp punktiem");
        }
    }
    void checkLocation(GameObject obj)
    {
        if(StartX > obj.transform.position.x && obj.transform.position.x > EndX)
        {
            isWithInX = true;
        }
        if(StartY > obj.transform.position.y && obj.transform.position.y > EndY)
        {
            isWithInY = true;
        }
        if (StartZ > obj.transform.position.z && obj.transform.position.z > EndZ)
        {
            isWithInZ = true;
        }
    }
}
