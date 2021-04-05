using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStartupEnableDisable : MonoBehaviour
{
    public List<GameObject> m_Objects;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("DoToggle");
    }

    private IEnumerator DoToggle()
    {
        yield return new WaitForSeconds(1f);
        foreach(GameObject obj in m_Objects)
        {
            obj.SetActive(false);
        }

        yield return new WaitForSeconds(0.3f);
        foreach (GameObject obj in m_Objects)
        {
            obj.SetActive(true);
        }
        yield return null;
    }
}
