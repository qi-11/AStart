using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 广度优先搜索（BFS）
/// </summary>
public class BFSPathFinder : PathFindBase
{
    protected override void DetectNeighborBlock(BlockItem detectBlock, BlockItem neighborBlock)
    {
        if (!list_finish.Contains(neighborBlock) && !queue_detect.Contains(neighborBlock))
        {
            //正在检测的区块
            neighborBlock.SetItemState(ItemState.Checking);
            float neighborDis = BlockManager.Instance.GetBlockDis(detectBlock, neighborBlock);
            neighborBlock.sumDistance = neighborDis + detectBlock.sumDistance;

            neighborBlock.priority=list_finish.Count;
            neighborBlock.preBlock = detectBlock;
            queue_detect.Enqueue(neighborBlock);

            //检测完显示方向信息
            neighborBlock.SetItemState(ItemState.DebugDir);
        }
    }
}
