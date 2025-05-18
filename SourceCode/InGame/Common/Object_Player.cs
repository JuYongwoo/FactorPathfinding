using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 시야를 사용하지 않고 모든 정보를 알 때의 플레이어의 행동
/// </summary>
public class Object_Player : Object
{

    public int mPlayerID = -1;
    public PathAlgorithm mUsingPathAlgorithm = null;
    public Queue<Vector2Int> mMovingGridQueue = new Queue<Vector2Int>();
    public bool mIsSteping = false;

    public Vector2Int mPreviousStartXY = new Vector2Int(-1, -1); //이전 시작위치, 내가 길찾기를 또 해야하나 체크하기 위해
    public Vector2Int mPreviousGoalXY = new Vector2Int(-1, -1); //이전 도착위치, 내가 길찾기를 또 해야하나 체크하기 위해

    public int mGotItemsCount = 0;
    public List<long> mCalculateTimes = new List<long>();
    public Vector2Int mFirstPositions;
    public const float mMoveSpeed = 10000.0f;
    protected JStopWatch mStopWatch = new JStopWatch();
    public Object_Player(int pPlayerID)
    {
        mPlayerID = pPlayerID;
    }
    public void initialize<T>() where T : PathAlgorithm, new()
    {
        mUsingPathAlgorithm = new T(); // T 타입의 새 인스턴스 생성
        mUsingPathAlgorithm.initialize(mPlayerID);
    }

    public void Update()
    {

        if (!mIsSteping)
        {
            checkCollision();//닿는 아이템들 제거
            mUsingPathAlgorithm.preProcessing(InGame_GameManager.mMap, mMovingGridQueue.Count != 0 ? true : false);

            if (mMovingGridQueue.Count != 0)
            {
                mPreviousGoalXY = mMovingGridQueue.Dequeue(); //큐가 비지 않았기 때문에 데큐
            }
            else
            {
                if (InGame_GameManager.mMap.mItems.Count > 0)
                {
                    if (mPlayerID == 0) mStopWatch.start();  //  && mGotItems<=6 지울 것

                    setDestination(mUsingPathAlgorithm.getFindResult(InGame_GameManager.mMap, mPlayerID));

                    if (mPlayerID == 0) mCalculateTimes.Add(mStopWatch.getTime(JStopWatch.TIME_UNIT.MICROSECOND));
                    //  && mGotItems<=6 지울 것
                }
                return;
            }
        }
        move();



    }
    public void move()
    {
        Vector3 destPos = InGame_GameManager.mMap.mGrids[mPreviousGoalXY.y, mPreviousGoalXY.x].mObject.transform.position;
        Vector3 moveDir = destPos - mObject.transform.position;

        // 도착 여부 체크
        float dist = moveDir.magnitude;
        if (dist < mMoveSpeed * Time.deltaTime) //이동 끝
        {
            if(mPlayerID == 0)InGame_GameManager.mMap.mTimeStep++;
            mNodePositionXY = InGame_GameManager.mMap.searchUnityXYToJVectorXY((int)destPos.x, (int)destPos.y);
            mIsSteping = false;
        }
        else // 이동 중
        {
            mObject.transform.position += moveDir.normalized * mMoveSpeed * Time.deltaTime;
            mIsSteping = true;
        }
    }
    public void checkCollision()// 목적지에 도착 했는가?
    {
        //도착한 아이템은 gameobject destroy하고 리스트에서 제외한다.
        for (int i = InGame_GameManager.mMap.mItems.Count - 1; i >= 0; i--)
        {
            if (InGame_GameManager.mMap.mItems[i] == this) continue; //나빼고 무언가와 닿았는가
            if (InGame_GameManager.mMap.mItems[i].mNodePositionXY == mNodePositionXY)
            {
                //InGame_GameManager.mMap.mItems[i].mObject.SetActive(false);
                UnityEngine.Object.Destroy(InGame_GameManager.mMap.mItems[i].mObject);
                InGame_GameManager.mMap.mItems.RemoveAt(i);
                mGotItemsCount++; //1개 획득 증가
            }
        }
    }


    public void setDestination(List<Vector2Int> pPathfindingResult)
    {
        for(int i = 0; i < pPathfindingResult.Count; i++)
        {
            mMovingGridQueue.Enqueue(pPathfindingResult[i]);
        }

        mPreviousStartXY = mNodePositionXY; //현재 시작지점 저장

    }




}
