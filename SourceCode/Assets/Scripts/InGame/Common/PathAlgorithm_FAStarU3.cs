using System;
using System.Collections.Generic;
using UnityEngine;

public class PathAlgorithm_FAStarU3 : PathAlgorithm
{
    protected int[,] mScoreBoard = new int[InGame_GameManager.mMap.mMapYsize, InGame_GameManager.mMap.mMapXsize];
    int mCount = 0;
    public bool mMustBeRefeshScoreBoard = false;

    public override void preProcessing(MapData pMap, bool pIsMoving) // Main
    {
        if (!pIsMoving)
        {
            //1단계 점수 설정(최초 1회)
            if (mNowMapID != pMap.mMapID)
            {
                mNowMapID = pMap.mMapID;
                mCount = 0;
            }
            if (mCount++ % 4 == 0) //탐색 2회 간격으로 safetyscore를 업데이트한다.
            {
                Array.Clear(mScoreBoard, 0, mScoreBoard.Length);

                mScoreBoard = StaticPathUtils.getScoreBoardwithBFSfromEnemiesCurrentPosition(pMap, mActorIndex);
            }
        }
        if (pIsMoving && mMustBeRefeshScoreBoard)
        {
            Array.Clear(mScoreBoard, 0, mScoreBoard.Length);

            mScoreBoard = StaticPathUtils.getScoreBoardwithBFSfromEnemiesCurrentPosition(pMap, mActorIndex);

        }

    }
    public override List<Vector2Int> getFindResult(MapData pMap, int pActorIndex) // Main
    {


        //목표 설정
        List<Vector2Int> lGoals = StaticPathUtils.getSafeItems(pMap, mScoreBoard, StaticVariables.sGameOverDistance);


        if (lGoals.Count == 0) // 유효한 아이템이 0이라면
        {

            lGoals = StaticPathUtils.getSafeItems(pMap, mScoreBoard, 0); //다시 아이템을 찾아본다
            //TODO JYW 여기서 가장 Score가 높은 Vision 외의 노드를 직접 리스트에 하나 추가해주면 끝.
            //2단계에서 시야 가장자리들을 IsBoundary로 체크해놓고 이것들을 리스트에 넣으면?????????????

        }

        //길찾기 시작
        List<Vector2Int> lItemsResults = StaticPathUtils.getPathwithAstar(pMap, pMap.mPlayers[mActorIndex].mNodePositionXY, lGoals, mScoreBoard);


        //무엇이 가치있는 길인가
        return lItemsResults;

    }

}
