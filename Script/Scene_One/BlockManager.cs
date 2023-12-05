using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public enum BlockState
{
    Prepare,
    SetPos,
    Runing,
    Over
}

public enum PathFindMode
{
    BreadthFirst,
    Dijkstar,
    Greedy,
    Astar
}

public class BlockManager : Singleton<BlockManager>
{
    int m_xCount;
    int m_yCount;

    public BlockState m_blockState;
    public BlockItem m_startBlock = null;
    public BlockItem m_endBlock = null;
    public int delay = 0;
    public PathFindMode m_pathFind;

    BlockItem[,] blockItems;

    public bool[,] stateArr;

    Vector2[] allDirs =
    {
        //顺时针方向
        new Vector2(-1,1),
        new Vector2(0,1),
        new Vector2(1,1),
        new Vector2(1,0),
        new Vector2(1,-1),
        new Vector2(0,-1),
        new Vector2(-1,-1),
        new Vector2(-1,0)

        //上 下 左 右  左上  右上  左下  右下
        //new Vector2(0,1),
        //new Vector2(0,-1),
        //new Vector2(-1,0),
        //new Vector2(1,0),
        //new Vector2(-1,1),
        //new Vector2(1,1),
        //new Vector2(-1,-1),
        //new Vector2(1,-1)
    };

    /// <summary>
    /// 初始化地图
    /// </summary>
    /// <param name="xCount"></param>
    /// <param name="yCount"></param>
    /// <param name="itemRoot"></param>
    public void Init(int xCount, int yCount, GameObject itemRoot, int delay, PathFindMode pathFindMode)
    {
        m_xCount = xCount;
        m_yCount = yCount;
        blockItems = new BlockItem[xCount, yCount];
        this.delay = delay;
        m_pathFind = pathFindMode;

        for (int i = 0; i < m_xCount; i++)
        {
            for (int j = 0; j < m_yCount; j++)
            {
                GameObject item = GameObject.Instantiate(Resources.Load<GameObject>("Item"), itemRoot.transform);
                BlockItem blockItem = item.GetComponent<BlockItem>();
                blockItem.Init(i, j);
                blockItems[i, j] = blockItem;
            }
        }
        m_blockState = BlockState.Prepare;//设置游戏状态为准备状态
    }

    /// <summary>
    /// 反初始化
    /// </summary>
    public void UnInit()
    {
        for (int i = 0; i < m_xCount; i++)
        {
            for (int j = 0; j < m_yCount; j++)
            {
                blockItems[i, j].UnInit();
            }
        }
    }

    /// <summary>
    /// 点击item
    /// </summary>
    /// <param name="blockItem"></param>
    public async void OnClickItem(BlockItem blockItem)
    {
        switch (m_blockState)
        {
            case BlockState.Prepare:
                m_blockState = BlockState.SetPos;
                m_startBlock = blockItem;
                m_endBlock = null;
                m_startBlock.SetItemState(ItemState.StartPos);
                break;
            case BlockState.SetPos:
                m_blockState = BlockState.Runing;
                m_endBlock = blockItem;
                m_endBlock.SetItemState(ItemState.EndPos);
                UpDataMapData();

                PathFindBase pathFindBase = null;
                switch (m_pathFind)
                {
                    case PathFindMode.BreadthFirst:
                        pathFindBase = new BFSPathFinder();
                        break;
                    case PathFindMode.Dijkstar:
                        pathFindBase = new DijkStarPathFinder();
                        break;
                    case PathFindMode.Greedy:
                        pathFindBase = new GreedyPathFinder();
                        break;
                    case PathFindMode.Astar:
                        pathFindBase = new AstarPathFinder();
                        break;
                    default:
                        break;
                }



                string idStr = "PathID";
                float startTime = Time.realtimeSinceStartup;
                List<BlockItem> list_path = await CalcPath(m_startBlock, m_endBlock, pathFindBase);
                for (int i = 0; i < list_path.Count; i++)
                {
                    list_path[i].SetItemState(ItemState.PathResult);
                    idStr += $"->{list_path[i]}";
                }
                Debug.Log($"{m_pathFind}\n Len:{m_endBlock.sumDistance}  Time:{Time.realtimeSinceStartup - startTime} ");

                m_endBlock.SetItemState(ItemState.EndPos);

                m_blockState = BlockState.Over;
                break;
            case BlockState.Runing:
                break;
            case BlockState.Over:
                break;
            default:
                break;
        }
    }
    

    /// <summary>
    /// 检测路径
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="pathFind"></param>
    /// <returns></returns>
    public Task<List<BlockItem>> CalcPath(BlockItem start, BlockItem end, PathFindBase pathFind)
    {
        return pathFind.CalcPath(start, end);
    }


    /// <summary>
    /// 检测地图
    /// </summary>
    public void UpDataMapData()
    {
        for (int i = 0; i < m_xCount; i++)
        {
            for (int j = 0; j < m_yCount; j++)
            {
                if (blockItems[i, j].m_walkable)
                {
                    blockItems[i, j].list_neighborBlock = GetNeighList(i, j);
                }
            }
        }
    }



    /// <summary>
    /// 获取item相邻的格子
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private List<BlockItem> GetNeighList(int x, int y)
    {
        List<BlockItem> list_temp = new List<BlockItem>();
        for (int i = 0; i < allDirs.Length; i++)
        {
            int _x = x + (int)allDirs[i].x;
            int _y = y + (int)allDirs[i].y;
            if (InsideBroder(_x, _y) && blockItems[_x, _y].m_walkable)
            {
                list_temp.Add(blockItems[_x, _y]);
            }
        }
        return list_temp;
    }

    private bool InsideBroder(int x, int y)
    {
        return x >= 0 && y >= 0 && x < m_xCount && y < m_yCount;
    }

    /// <summary>
    /// 斜边为浮点数
    /// </summary>
    /// <param name="start"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public float GetBlockDis(BlockItem start, BlockItem target)
    {
        float x = MathF.Abs(target.m_xIndex - start.m_xIndex);
        float y = MathF.Abs(target.m_yIndex - start.m_yIndex);
        float min = MathF.Min(x, y);
        float max = MathF.Max(x, y);
        return min * 1.4f + (max - min);
    }

    /// <summary>
    /// 斜边为整数
    /// </summary>
    /// <param name="start"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public float GetBlockDisManhattan(BlockItem start, BlockItem target)
    {
        float x = MathF.Abs(target.m_xIndex - start.m_xIndex);
        float y = MathF.Abs(target.m_yIndex - start.m_yIndex);
        return x + y;
    }


    /// <summary>
    /// 保存每个格子的数据
    /// </summary>
    public void SaveWalkableState()
    {
        stateArr = new bool[m_xCount, m_yCount];
        for (int i = 0; i < m_xCount; i++)
        {
            for (int k = 0; k < m_yCount; k++)
            {
                stateArr[i, k] = blockItems[i, k].m_walkable;
            }
        }
    }

    /// <summary>
    /// 刷新每个格子的数据
    /// </summary>
    public void RecoverWalkableState()
    {
        if (stateArr != null)
        {
            for (int i = 0; i < m_xCount; i++)
            {
                for (int k = 0; k < m_yCount; k++)
                {
                    blockItems[i, k].SetItemWalkState(stateArr[i, k]);
                }
            }
            stateArr = null;
        }

    }

    /// <summary>
    /// 重置起始点和终止点
    /// </summary>
    /// <param name="pathFind"></param>
    public void ResetPos(PathFindMode pathFind)
    {
        if (m_blockState == BlockState.Over)
        {
            m_pathFind = pathFind;

            SaveWalkableState();
            UnInit();

            m_blockState = BlockState.Prepare;
            RecoverWalkableState();
        }
    }


    /// <summary>
    /// 刷新
    /// </summary>
    /// <param name="pathFind"></param>

    public void RefreshMap(PathFindMode pathFind)
    {
        if (m_blockState == BlockState.Over)
        {
            m_pathFind = pathFind;
            UnInit();

            m_blockState = BlockState.Prepare;
            

            
        }
    }
    /// <summary>
    /// 回放
    /// </summary>
    /// <param name="pathFind"></param>
    public void PlaybackMap(PathFindMode pathFind)
    {
        if (m_blockState == BlockState.Over)
        {
            m_pathFind = pathFind;

            SaveWalkableState();
            UnInit();

            m_blockState = BlockState.Prepare;
            RecoverWalkableState();

            BlockItem tempEnd = m_endBlock;
            OnClickItem(m_startBlock);
            OnClickItem(tempEnd);
        }
    }

}
