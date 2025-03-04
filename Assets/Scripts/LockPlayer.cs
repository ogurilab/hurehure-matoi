using System.Collections.Generic;
using UnityEngine;

public class LockObjects : MonoBehaviour
{
    private List<Transform> lockedObjects = new List<Transform>();
    private List<Vector3> lockedPositions = new List<Vector3>();

    // エリアにオブジェクトが入ったとき
    private void OnTriggerEnter(Collider other)
    {
        if (!lockedObjects.Contains(other.transform))
        {
            lockedObjects.Add(other.transform);
            lockedPositions.Add(other.transform.position);
        }
    }

    // エリアからオブジェクトが出たとき
    private void OnTriggerExit(Collider other)
    {
        int index = lockedObjects.IndexOf(other.transform);
        if (index != -1)
        {
            lockedObjects.RemoveAt(index);
            lockedPositions.RemoveAt(index);
        }
    }

    void Update()
    {
        for (int i = 0; i < lockedObjects.Count; i++)
        {
            if (lockedObjects[i] != null)
            {
                lockedObjects[i].position = lockedPositions[i];
            }
        }
    }
}
