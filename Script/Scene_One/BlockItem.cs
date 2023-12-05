using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ItemState
{
    Walk,
    Block,
    StartPos,
    EndPos,
    Checking,
    DebugDir,
    PathResult
}

public class BlockItem : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IComparable<BlockItem>
{
    public int m_xIndex;
    public int m_yIndex;
    public bool m_walkable;

    public Image m_image_state;
    public Text m_text_dis;
    public Text m_text_pos;
    public GameObject m_arrow;

    /// <summary>
    /// 存储此格子的所有相邻格子
    /// </summary>
    public List<BlockItem> list_neighborBlock;

    /// <summary>
    /// 当前区块的前一个区块
    /// </summary>
    public BlockItem preBlock;

    /// <summary>
    /// 记录从起点到当前区块的总距离长度
    /// float.PositiveInfinity  无穷大
    /// </summary>
    public float sumDistance = float.PositiveInfinity;


    /// <summary>
    /// 计算优先级
    /// </summary>
    public float priority;


    /// <summary>
    /// 初始化格子信息
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void Init(int x, int y)
    {
        
        m_xIndex = x;
        m_yIndex = y;
        m_walkable = true;
        gameObject.name = $"block_{m_xIndex},{m_yIndex}";
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        rect.localScale = Vector3.one;
        rect.anchoredPosition = new Vector2(m_xIndex * 100, m_yIndex * 100);
        m_text_pos.text = $"{m_xIndex},{m_yIndex}";
        SetItemState(ItemState.Walk);
    }

    public void UnInit()
    {
        m_walkable = true;
        preBlock = null;
        sumDistance=float.PositiveInfinity;

        SetItemState(ItemState.Walk);
    }


    /// <summary>
    /// 设置格子状态
    /// </summary>
    /// <param name="state"></param>
    public void SetItemState(ItemState state)
    {
        switch (state)
        {
            case ItemState.Walk:
                m_image_state.color = Color.white;
                m_text_dis.gameObject.SetActive(false);
                m_arrow.SetActive(false);
                break;
            case ItemState.Block:
                m_image_state.color = Color.black;
                m_text_dis.gameObject.SetActive(false);
                m_arrow.SetActive(false);
                break;
            case ItemState.StartPos:
                m_image_state.color = Color.blue;
                break;
            case ItemState.EndPos:
                m_image_state.color = Color.red;
                break;
            case ItemState.Checking:
                m_image_state.color = Color.magenta;
                m_text_dis.gameObject.SetActive(true);
                m_arrow.SetActive(false);
                m_text_dis.text = sumDistance.ToString();
                break;
            case ItemState.DebugDir:
                ShowSearchDir(Color.yellow);
                m_text_dis.text = sumDistance.ToString();
                break;
            case ItemState.PathResult:
                ShowSearchDir(Color.green);
                m_text_dis.text = sumDistance.ToString();
                break;
            default:
                break;
        }
    }

    private void ShowSearchDir(Color color)
    {
        if (preBlock)
        {
            m_image_state.color = color;
            m_arrow.SetActive(true);
            int deltX = m_xIndex - preBlock.m_xIndex;
            int deltY = m_yIndex - preBlock.m_yIndex;
            float angle = Vector3.SignedAngle(Vector3.right,new Vector3(deltX,deltY,0),Vector3.forward);
            m_arrow.transform.localEulerAngles = new Vector3(0,0,angle);
            
        }
        else
        {
            m_arrow.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Input.GetMouseButton(1))
        {
            if (BlockManager.Instance.m_blockState == BlockState.Prepare)
            {
                SetItemWalkState(!m_walkable);
            } 
        }
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Input.GetMouseButtonDown(0))//左键点击设置起始点和终止点
        {
            if (m_walkable)
            {
                BlockManager.Instance.OnClickItem(this);
            }
        }
        else
        {
            if (BlockManager.Instance.m_blockState==BlockState.Prepare)
            {
                SetItemWalkState(!m_walkable);
            }
        }
    }

    /// <summary>
    /// 设置可行走或不可行走
    /// </summary>
    /// <param name="state"></param>
    public void SetItemWalkState(bool state)
    {
        m_walkable = state;
        SetItemState(state? ItemState.Walk:ItemState.Block);
    }

    public int CompareTo(BlockItem other)
    {
        if (priority<other.priority)
        {
            return -1;
        }
        else if (priority>other.priority)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}
