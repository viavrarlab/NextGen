using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Placeable : MonoBehaviour
{
    public int m_ID = 0;
    public bool m_IsPlaced = false;

    public Placeable(int _id)
    {
        m_ID = _id;
    }

    public Placeable()
    {

    }

    /// <summary>
    /// Works kind of like a constructor. When in need to add the component through script to a GO, we call this instead of GO.AddComponent<>(). This allows us to set the ID straight on component add, instead of adding it after adding and getting the component.
    /// </summary>
    /// <param name="_where"></param>
    /// <param name="_id"></param>
    /// <returns></returns>
    public static Placeable CreateComponentReturn(GameObject _where, int _id)
    {
        Placeable myC = _where.AddComponent<Placeable>();
        myC.m_ID = _id;
        return myC;
    }

    public static void CreateComponent(GameObject _where, int _id)
    {
        Placeable myC = _where.AddComponent<Placeable>();
        myC.m_ID = _id;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    /// <summary>
    /// Gets called when the script gets added to a GameObject. Set the objects tag and layer.
    /// </summary>
    void Reset()
    {
        gameObject.tag = "Grab";
        gameObject.layer = LayerMask.NameToLayer("Placeable");
    }
}
