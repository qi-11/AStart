using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreedyPathFinder : PathFindBase
{


    
    protected override void DetectNeighborBlock(BlockItem detectBlock, BlockItem neighborBlock)
    {
        if (!list_finish.Contains(neighborBlock) && !queue_detect.Contains(neighborBlock))
        {
            //正在检测的区块
            neighborBlock.SetItemState(ItemState.Checking);
            float neighborDis = BlockManager.Instance.GetBlockDis(detectBlock, neighborBlock);
            neighborBlock.sumDistance = neighborDis + detectBlock.sumDistance;
            
            neighborBlock.priority = BlockManager.Instance.GetBlockDis(neighborBlock, m_endBlock);
            neighborBlock.preBlock = detectBlock;
            queue_detect.Enqueue(neighborBlock);

            //检测完显示方向信息
            neighborBlock.SetItemState(ItemState.DebugDir);
        }
    }

    
}
