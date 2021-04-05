using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class DotweenTest : MonoBehaviour
{
    public Vector3[] path = new Vector3[2];
    // Start is called before the first frame update
    void Start ()
    {

    }

    // Update is called once per frame
    void Update ()
    {

    }
[Button]
    public void MOOOOOOve ()
    {
        transform.DOPath (path, 5f, PathType.CubicBezier);
    }
}