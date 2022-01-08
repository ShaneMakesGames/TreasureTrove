using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    public List<Object> ActiveObjects = new List<Object>();
    public List<Object> ObjectPool = new List<Object>();

    public void NextObjectInPool()
    {
        // Gets position of object furthest in front
        Vector3 newObjectPos = ActiveObjects[2].transform.position;

        // Disables wall behind you
        Object objToDisable = ActiveObjects[0];
        // Any code necessary for enabling the object
        objToDisable.DisableObject();
        // Moves object from active list to pool
        ActiveObjects.Remove(objToDisable);
        ObjectPool.Add(objToDisable);

        // Enables wall in front of you
        Object objToEnable = ObjectPool[0];
        // Any code necessary for enabling the object
        objToEnable.EnableObject(newObjectPos);
        // Moves object from pool to active list
        ObjectPool.Remove(objToEnable);
        ActiveObjects.Add(objToEnable);

        // Sets object in middle as the active trigger (assumes there are 3 active objects at a time)
        // This is used for endless games, the middle object's trigger calls NextObjectInPool() 
        ActiveObjects[1].SetTriggerActive(true);
    }
}
