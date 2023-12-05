using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class PathFindBase 
{
    /// <summary>
    /// 等待检测的队列
    /// </summary>
    protected PEQue<BlockItem> queue_detect=new PEQue<BlockItem>();

    /// <summary>
    /// 搜索完成的列表
    /// </summary>
    protected List<BlockItem> list_finish=new List<BlockItem>();

    /// <summary>
    /// 路径最终路径的列表
    /// </summary>
    protected List<BlockItem> list_overPath=new List<BlockItem>();


    protected BlockItem m_startBlock;
    protected BlockItem m_endBlock;

    public async Task<List<BlockItem>> CalcPath(BlockItem start, BlockItem end)
    {
        m_startBlock=start;
        m_endBlock=end;
        queue_detect.Clear();
        list_finish.Clear();
        list_overPath.Clear();
        m_startBlock.sumDistance = 0;

        queue_detect.Enqueue(m_startBlock);

        while (queue_detect.Count > 0)
        {
            if (queue_detect.Contains(m_endBlock))
            {

                list_overPath = GetPath();

                break;
            }

            BlockItem detectBlock = queue_detect.Dequeue();
            if (!list_finish.Contains(detectBlock))
            {
                list_finish.Add(detectBlock);
            }

            for (int i = 0; i < detectBlock.list_neighborBlock.Count; i++)
            {
                BlockItem neighborBlock = detectBlock.list_neighborBlock[i];
                await Task.Delay(BlockManager.Instance.delay);
                DetectNeighborBlock(detectBlock, neighborBlock);
            }

        }

        return list_overPath;
    }

    protected abstract void DetectNeighborBlock(BlockItem detectBlock, BlockItem neighborBlock);


    protected List<BlockItem> GetPath()
    {
        List <BlockItem> list_temp=new List<BlockItem>();
        BlockItem curBlock = m_endBlock;
        while (curBlock.preBlock != null)
        {
            list_temp.Insert(0,curBlock);
            curBlock=curBlock.preBlock;
        }
        return list_temp;
    }
}
