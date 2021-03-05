using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class MagneticFieldManager : MonoBehaviour
{
    [Tag]
    public string m_PositiveFieldTag;
    [Tag]
    public string m_NegativeFieldTag;


    public static MagneticFieldManager instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
