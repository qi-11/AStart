using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject players;
    public GameObject monsters;
    
    public List<GameObject> list_players;
    public List<GameObject> list_monster;

    public List<GameObject> list_team;//当前进行操作的队伍

    // Start is called before the first frame update
    void Start()
    {
        list_players = new List<GameObject>();
        list_monster = new List<GameObject>();
        list_team = new List<GameObject>();
        for (int i = 0; i < players.transform.childCount; i++)
        {
            list_players.Add(players.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < monsters.transform.childCount; i++)
        {
            list_monster.Add(players.transform.GetChild(i).gameObject);
        }
        list_team = list_players;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
