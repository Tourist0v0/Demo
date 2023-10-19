using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    private bool isGameOver;
    // 开一个字典来存储每个玩家名字与玩家的映射关系
    private Dictionary<string, Player> players = new Dictionary<string, Player>();
    private static string info;
    // 开一个字典来存储每个哨兵名字与哨兵的映射关系
    private Dictionary<string, Sentry> sentries = new Dictionary<string, Sentry>();
    // 开一个字典来存储每个基地名字与哨兵的映射关系
    private Dictionary<string, Tower> towers = new Dictionary<string, Tower>();
    public static GameManager Singleton;

    private void Update()
    {
        foreach (string name in towers.Keys)
        {
            Tower tower = GetTower(name);
            if (tower.GetDeadState()) isGameOver = true;
        }

        if (isGameOver)
        {
            foreach (string name in players.Keys)
            {
                Player player = GetPlayer(name);
                player.GameOver();
            }
        }
    }

    public static void UpdateInfo(string _info)
    {
        info = _info;
    }

    private void Awake()
    {
        Singleton = this;
    }
    // 加入一个玩家
    public void RegisterPlayer(string name, Player player)
    {
        player.transform.name = name;
        players.Add(name, player);
        player.gameObject.layer = players.Count;
    }
    // 删除一个玩家
    public void UnRegisterPlayer(string name)
    {
        players.Remove(name);
    }
    // 加入一个哨兵
    public void RegisterSentry(string name, Sentry sentry)
    {
        sentry.transform.name = name;
        sentries.Add(name, sentry);
    }
    // 删除一个哨兵
    public void UnRegisterSentry(string name)
    {
        sentries.Remove(name);
    }
    // 加入一个基地
    public void RegisterTower(string name, Tower tower)
    {
        tower.transform.name = name;
        towers.Add(name, tower);
    }
    // 删除一个基地
    public void UnRegisterTower(string name)
    {
        towers.Remove(name);
    }    
    // 根据基地名字返回基地
    public Tower GetTower(string name)
    {
        return towers[name];
    }
    // 根据哨兵名字返回哨兵
    public Sentry GetSentry(string name)
    {
        return sentries[name];
    }
    // 根据玩家名字返回玩家
    public Player GetPlayer(string name)
    {
        return players[name];
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(200f,200f,200,400));
        GUILayout.BeginVertical();
        GUI.color = Color.red;
        foreach (string name in players.Keys)
        {
            Player player = GetPlayer(name);
            GUILayout.Label(name + " : " + player.GetCurrentHealth());
        }

        foreach (string name in sentries.Keys)
        {
            Sentry sentry = GetSentry(name);
            GUILayout.Label(name + " : " + sentry.GetCurrentHealth());
        }

        foreach (string name in towers.Keys)
        {
            Tower tower = GetTower(name);
            GUILayout.Label(name + " : " + tower.GetCurrentHealth());
        }
        GUILayout.EndArea();
        GUILayout.EndVertical();
    }
}
