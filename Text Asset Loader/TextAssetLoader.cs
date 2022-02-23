using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct EnemyStats
{
    public string Name;
    public int Health;
    public int Damage;
    public int MoveSpeed;
}
public class TextAssetLoader : MonoBehaviour
{
    public Dictionary<string, EnemyStats> EnemyStatDict = new Dictionary<string, EnemyStats>();
    public const string path = "EnemyData";
    void Awake()
    {
        LoadEnemyStatsFromTextFile();
    }

    public void LoadEnemyStatsFromTextFile()
    {
        TextAsset textAsset = GetTextAsset(path);
        if (textAsset != null)
        {
            string[] data = textAsset.text.Split(new char[] { '\n' });
            for (int i = 1; i < data.Length; i++)
            {
                EnemyStats enemyStats;
                string[] row = data[i].Split(new char[] { '\t' });
                enemyStats.Name = row[0];
                enemyStats.Health = int.Parse(row[1]);
                enemyStats.Damage = int.Parse(row[2]);
                enemyStats.MoveSpeed = int.Parse(row[3]);
                EnemyStatDict.Add(enemyStats.Name, enemyStats);
            }
        }
    }

    public TextAsset GetTextAsset(string path)
    {
        TextAsset textAsset = Resources.Load(path) as TextAsset;
        if (textAsset != null)
        {
            return textAsset;

        }
        else
        {
            return null;
        }
    }
}