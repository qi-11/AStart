using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRoot : MonoBehaviour
{
    public GameObject itemRoot;
    public int xCount;
    public int yCount;
    public int delay ;
    public PathFindMode pathFindMode;
    // Start is called before the first frame update
    void Start()
    {
        //≥ı ºªØµÿÕº
        BlockManager.Instance.Init(xCount,yCount,itemRoot,delay, pathFindMode);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            
            BlockManager.Instance.PlaybackMap(pathFindMode);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            BlockManager.Instance.ResetPos(pathFindMode);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            BlockManager.Instance.RefreshMap(pathFindMode);
        }
    }
}
