using System.Collections.Generic;
using UnityEngine;

/*
 * initialize로 플레이어, 목표 위치와 그리드 정보를 받아 계산하여 계산된 플레이어 위치를 onRender로 반환해 주는 길찾기 모듈
 * 
 * 
 */

public class PathAlgorithm_BFS : PathAlgorithm
{

    public override List<Vector2Int> getFindResult(MapData pMap, int pActorIndex) // Main
    {


        //목표 설정
        List<Vector2Int> lGoals = new List<Vector2Int>(StaticPathUtils.getAllItems(pMap));

        //길찾기 시작
        List<Vector2Int> lItemsResults = StaticPathUtils.getPathwithBFS(pMap, pMap.mPlayers[mActorIndex].mNodePositionXY, lGoals);


        //무엇이 가치있는 길인가
        return lItemsResults;
    }


}
