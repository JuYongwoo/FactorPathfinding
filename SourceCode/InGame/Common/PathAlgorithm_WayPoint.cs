using System;
using System.Collections.Generic;
using UnityEngine;

public class PathAlgorithm_WayPoint : PathAlgorithm // TODO Count-1,Count-1 -> Count-1,0 -> 0,0 -> 0,Count-1, 반복하도록 수정
{
    private List<Vector2Int> mWayPoints;


    public override List<Vector2Int> getFindResult(MapData pMap, int pActorIndex) // Main
    {

        //맵이 바뀔 때마다
        if (mNowMapID != pMap.mMapID)
        {
            mNowMapID = pMap.mMapID;

            mWayPoints = new List<Vector2Int>();
            mWayPoints.Add(new Vector2Int(0, 0));
            mWayPoints.Add(new Vector2Int(pMap.mMapYsize - 1, pMap.mMapXsize - 1));
        }

        List<Vector2Int> lGoals = new List<Vector2Int>(mWayPoints);

        //길찾기 시작
        List<Vector2Int> lItemsResults = StaticPathUtils.getPathwithAstar(pMap, pMap.mPlayers[mActorIndex].mNodePositionXY, lGoals);


        //무엇이 가치있는 길인가
        return lItemsResults;

    }

}
