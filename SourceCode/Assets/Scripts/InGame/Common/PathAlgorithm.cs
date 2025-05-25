using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * initialize로 플레이어, 목표 위치와 그리드 정보를 받아 계산하여 계산된 플레이어 위치를 onRender로 반환해 주는 길찾기 모듈
 * 
 * 
 */

public class PathAlgorithm
{

    protected int mNowMapID = -1;
    protected int mActorIndex = -1;


    public void initialize(int pActorIndex)
    {
        mActorIndex = pActorIndex;


    }
    public virtual void preProcessing(MapData pMap, bool pIsMoving) // Main
    {
        return;

    }
    public virtual List<Vector2Int> getFindResult(MapData pMap, int pActorIndex) // Main
    {

        return null;

    }
}
