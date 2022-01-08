using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeilingTrigger : MonoBehaviour
{
    public bool isTriggerActive;
    public WallPooling wallPooling;

    public List<Enemy> Enemies = new List<Enemy>();

    public void OnTriggerEnter(Collider other)
    {
        if (isTriggerActive)
        {
            if (other.CompareTag("Player"))
            {
                wallPooling.NextWallInPool();
                SetTriggerActive(false);
            }
        }
    }

    public void DisableWall()
    {
        gameObject.SetActive(false);
    }

    public void EnableWall(Vector3 wallPos)
    {
        gameObject.SetActive(true);
        wallPos.y += 15;
        transform.position = wallPos;

        if (Enemies.Count < 0) return;
        for (int i = 0; i < Enemies.Count; i++)
        {
            Enemies[i].gameObject.SetActive(true);
        }
    }

    public void SetTriggerActive(bool isActive)
    {
        isTriggerActive = isActive;
    }
}
