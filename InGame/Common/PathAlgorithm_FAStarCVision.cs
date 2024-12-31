using System;
using System.Collections.Generic;
using UnityEngine;

public class PathAlgorithm_FAStarCVision : PathAlgorithm
{
    protected int[,] mScoreBoard = null;
    protected List<Vector2Int> mVisionNodeList; //true면 여태까지의 시야로 인해 정체가 밝혀진 부분
    protected List<Vector2Int> mBoundaryNodeList; //true면 시야값의 경계선
    public override void preProcessing(MapData pMap, bool pIsMoving) // Main
    {
        if (!pIsMoving)
        {
            //1단계 점수 설정(최초 1회)
            if (mNowMapID != pMap.mMapID)
            {
                mNowMapID = pMap.mMapID;
                if (mVisionNodeList == null) mVisionNodeList = new List<Vector2Int>();
                else mVisionNodeList.Clear(); // 기존 리스트를 비움

                if (mBoundaryNodeList == null) mBoundaryNodeList = new List<Vector2Int>();
                else mBoundaryNodeList.Clear(); // 기존 리스트를 비움

                if (mScoreBoard == null) mScoreBoard = new int[pMap.mMapYsize, pMap.mMapXsize];
                else Array.Clear(mScoreBoard, 0, mScoreBoard.Length);
                mScoreBoard = StaticPathUtils.getScoreBoardwithBFSfromPlayerCurrentPosition(pMap, mActorIndex, mScoreBoard); //시작 위치만 알기 때문에 시작 위치부터
            }
        }

    }
    public override List<Vector2Int> getFindResult(MapData pMap, int pActorIndex) // Main
    {
        


        // 목표 설정
        List<Vector2Int> lGoals = new List<Vector2Int>(getVisionGoals(pMap, pActorIndex));//유효한 아이템들을 찾는다. (시야에 있거나 위험지역 밖에 있거나)


        //길찾기 시작
        List<Vector2Int> lItemsResults = StaticPathUtils.getPathwithAstar(pMap, pMap.mPlayers[mActorIndex].mNodePositionXY, lGoals, mScoreBoard);

        
        //무엇이 가치있는 길인가
        return lItemsResults;

    }
    List<Vector2Int> getVisionGoals(MapData pMap, int pActorIndex)
    {
        // 시야 처리
        VisionBoundaryBoard lVisionBoundaryBoard = StaticPathUtils.getVisionedNodes(pMap, pMap.mPlayers[pActorIndex].mNodePositionXY, StaticVariables.sVisionDistance);

        // HashSet으로 중복 체크 및 추가 처리
        HashSet<Vector2Int> visionNodeSet = new HashSet<Vector2Int>(mVisionNodeList);

        foreach (var node in lVisionBoundaryBoard.mVisionNodeList)
        {
            if (visionNodeSet.Add(node)) // 중복이 없을 때만 추가
            {
                mVisionNodeList.Add(node);
            }
        }

        // 경계 노드 처리
        HashSet<Vector2Int> boundaryNodeSet = new HashSet<Vector2Int>(mBoundaryNodeList);

        foreach (var node in lVisionBoundaryBoard.mBoundaryNodeList)
        {
            if (boundaryNodeSet.Add(node)) // 중복이 없을 때만 추가
            {
                mBoundaryNodeList.Add(node);
            }
        }

        // 경계 노드 중에서 시야 안에 있는 노드 제거
        for (int i = mBoundaryNodeList.Count - 1; i >= 0; i--)
        {
            if (visionNodeSet.Contains(mBoundaryNodeList[i]))
            {
                mBoundaryNodeList.RemoveAt(i);
            }
        }

        // 목표 설정
        List<Vector2Int> lGoals = new List<Vector2Int>();

        foreach (var item in pMap.mItems)
        {
            if (visionNodeSet.Contains(item.mNodePositionXY))
            {
                lGoals.Add(item.mNodePositionXY);
            }
        }

        // 목표가 없으면 경계선 노드로 설정
        if (lGoals.Count == 0)
        {
            lGoals = new List<Vector2Int>(mBoundaryNodeList); // 경계에 있는 노드가 목표
        }

        return lGoals;

    }

}
