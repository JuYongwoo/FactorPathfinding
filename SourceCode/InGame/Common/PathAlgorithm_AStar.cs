using System;
using System.Collections.Generic;
using UnityEngine;

public class PathAlgorithm_AStar : PathAlgorithm
{


    public override List<Vector2Int> getFindResult(MapData pMap, int pActorIndex) // Main
    {

        //목표 설정
        List<Vector2Int> lGoals = new List<Vector2Int>(StaticPathUtils.getAllItems(pMap));

        //길찾기 시작
        List<Vector2Int> lItemsResults = StaticPathUtils.getPathwithAstar(pMap, pMap.mPlayers[mActorIndex].mNodePositionXY, lGoals);



        //무엇이 가치있는 길인가
        return lItemsResults;

    }



}
