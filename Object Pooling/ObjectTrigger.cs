using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTrigger : MonoBehaviour
{
    public bool isTriggerActive;
    public float offset;
    public ObjectPooling objectPooling;

    public void OnTriggerEnter(Collider other)
    {
        if (isTriggerActive)
        {
            if (other.CompareTag("Player"))
            {
                objectPooling.NextObjectInPool();
                SetTriggerActive(false);
            }
        }
    }

    public void DisableWall()
    {
        gameObject.SetActive(false);
    }

    public void EnableWall(Vector3 objPos)
    {
        gameObject.SetActive(true);
        wallPos.y += offset;
        transform.position = objPos;
    }

    public void SetTriggerActive(bool isActive)
    {
        isTriggerActive = isActive;
    }
}
