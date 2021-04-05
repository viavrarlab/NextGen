using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class ObjectScatterer : MonoBehaviour
{
    public Transform m_TargetObject;
    public Transform m_ScatterStartTransform;

    public float m_ScatterXAxisOffset = 0.25f;
    public float m_ScatterZAxisOffset = 0.04f;

    public List<Vector3> m_OriginalPositions = new List<Vector3>();
    public List<Quaternion> m_OriginalRotations = new List<Quaternion>();


    public void PerformScatter()
    {
        m_OriginalPositions.Clear();
        m_OriginalRotations.Clear();
        foreach (Transform t in m_TargetObject)
        {
            if (t.gameObject.name.Contains("Sockets")) { continue; }
            m_OriginalPositions.Add(t.position);
            m_OriginalRotations.Add(t.rotation);
        }
        ScatterObjects();
    }


    public void PerformReset()
    {
        int i = 0;
        foreach (Transform t in m_TargetObject)
        {
            if (t.gameObject.name.Contains("Sockets")) { continue; }
            t.position = m_OriginalPositions[i];
            t.rotation = m_OriginalRotations[i];
            i++;
        }
    }

    private void ScatterObjects()
    {
        Dictionary<GameObject, float> objectVolumeDict = new Dictionary<GameObject, float>();
        foreach (Transform t in m_TargetObject)
        {
            if (t.gameObject.name.Contains("Sockets")) { continue; }
            objectVolumeDict.Add(t.gameObject, VolumeOfMesh(t.gameObject.GetComponent<MeshFilter>().sharedMesh));
        }

        Dictionary<GameObject, float> sortedByVolume = objectVolumeDict.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

        Vector3 startPos = m_ScatterStartTransform.position;

        Vector3 placePos = startPos;
        int row = 1;
        for (int i = 0; i < sortedByVolume.Count; i++)
        {
            GameObject curObj = sortedByVolume.ElementAt(i).Key;

            Bounds objectMeshBounds = curObj.GetComponent<MeshRenderer>().bounds;
            RotateObjectToRestPosition(curObj.transform, objectMeshBounds);

            placePos.y =  m_ScatterStartTransform.position.y + curObj.GetComponent<MeshRenderer>().bounds.size.y / 2f;

            if (i % 5 == 0 && i != 0)
            {
                placePos.x = startPos.x;
                float moveZAmount = (row) * m_ScatterZAxisOffset;
                placePos.z += (moveZAmount);
                row++;
            }
            curObj.transform.position = placePos;
            placePos.x += m_ScatterXAxisOffset;
        }
    }

    public float VolumeOfMesh(Mesh mesh)
    {
        Vector3 m = mesh.bounds.size;
        return m.x * m.y * m.z;
    }

    private void RotateObjectToRestPosition(Transform obj, Bounds bounds)
    {
        List<float> sizeList = new List<float>();
        sizeList.Add(bounds.size.x);
        sizeList.Add(bounds.size.y);
        sizeList.Add(bounds.size.z);

        float max = Mathf.Max(sizeList.ToArray());
        int index = sizeList.IndexOf(max);

        Vector3 rotationAxis = Vector3.zero;
        switch (index)
        {
            case 0:
                float xY = (bounds.size.y > bounds.size.z) ? 1f : 0f;
                rotationAxis = new Vector3(1f, 0f, 0f);

                Quaternion rotX = Quaternion.LookRotation(rotationAxis, Vector3.up);
                if (xY > 0f) { rotX.eulerAngles += new Vector3(90f, 0f, 0f); }
                obj.rotation = rotX;
                break;

            case 1:
                float yX = (bounds.size.x > bounds.size.z) ? 0f : 1f;
                float z = (bounds.size.x > bounds.size.z) ? 0f : 1f;
                rotationAxis = new Vector3(0f, 1f, 0f);
                Quaternion rotY = Quaternion.LookRotation(rotationAxis, Vector3.up);
                if (yX > 0f) { rotY.eulerAngles = new Vector3(rotY.eulerAngles.x + 90f, rotY.eulerAngles.y, 0f); }
                obj.rotation = rotY;
                break;
            case 2:
                float zY = (bounds.size.x > bounds.size.y) ? 0f : 1f;
                rotationAxis = new Vector3(0f, 0f, 1f);
                Quaternion rotZ = Quaternion.LookRotation(rotationAxis, Vector3.up);
                if (zY > 0f) { rotZ.eulerAngles += new Vector3(0f, 0f, 90f); }
                obj.rotation = rotZ;
                break;
            default:
                break;
        }
    }
}
