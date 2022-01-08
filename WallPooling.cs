using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPooling : MonoBehaviour
{
    public List<CeilingTrigger> ActiveWalls = new List<CeilingTrigger>();
    public List<CeilingTrigger> WallPool = new List<CeilingTrigger>();

    public void NextWallInPool()
    {
        // Gets position of wall furthest in front
        Vector3 newWallPos = ActiveWalls[2].transform.position;

        // Disables wall behind you
        CeilingTrigger wallToDisable = ActiveWalls[0];
        wallToDisable.DisableWall();
        ActiveWalls.Remove(wallToDisable);
        WallPool.Add(wallToDisable);

        // Enables wall in front of you
        CeilingTrigger wallToEnable = WallPool[0];
        wallToEnable.EnableWall(newWallPos);
        WallPool.Remove(wallToEnable);
        ActiveWalls.Add(wallToEnable);

        // Sets wall in middle as active trigger
        ActiveWalls[1].SetTriggerActive(true);
    }
}