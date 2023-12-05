using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarPathFinder : PathFindBase
{
    protected override void DetectNeighborBlock(BlockItem detectBlock, BlockItem neighborBlock)
    {
        if (!list_finish.Contains(neighborBlock))
        {
            //正在检测的区块
            neighborBlock.SetItemState(ItemState.Checking);

            float neighborDis = BlockManager.Instance.GetBlockDis(detectBlock, neighborBlock);
            float newSumDistance = neighborDis + detectBlock.sumDistance;

            if (float.IsPositiveInfinity(neighborBlock.sumDistance) || neighborBlock.sumDistance > newSumDistance)
            {
                neighborBlock.sumDistance = newSumDistance;
                neighborBlock.preBlock = detectBlock;
            }

            if (!queue_detect.Contains(neighborBlock))
            {
                float targetDis = BlockManager.Instance.GetBlockDis(neighborBlock, m_endBlock);

                neighborBlock.priority = neighborBlock.sumDistance+ targetDis;
                queue_detect.Enqueue(neighborBlock);
            }


            //检测完显示方向信息
            neighborBlock.SetItemState(ItemState.DebugDir);
        }
    }

    
}
