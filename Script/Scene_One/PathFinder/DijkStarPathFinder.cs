using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �Ͽ�˹�����㷨��DijkStar��
/// </summary>
public class DijkStarPathFinder : PathFindBase
{
    protected override void DetectNeighborBlock(BlockItem detectBlock, BlockItem neighborBlock)
    {
        if (!list_finish.Contains(neighborBlock))
        {
            //���ڼ�������
            neighborBlock.SetItemState(ItemState.Checking);
            float neighborDis = BlockManager.Instance.GetBlockDis(detectBlock, neighborBlock);
            float newSumDistance= neighborDis + detectBlock.sumDistance;

            if (float.IsPositiveInfinity(neighborBlock.sumDistance) || neighborBlock.sumDistance> newSumDistance)
            {
                neighborBlock.sumDistance= newSumDistance;
                neighborBlock.preBlock = detectBlock;
            }

            if (!queue_detect.Contains(neighborBlock))
            {
                neighborBlock.priority = neighborBlock.sumDistance;
                queue_detect.Enqueue(neighborBlock);
            }
            

            //�������ʾ������Ϣ
            neighborBlock.SetItemState(ItemState.DebugDir);
        }
    }
}
