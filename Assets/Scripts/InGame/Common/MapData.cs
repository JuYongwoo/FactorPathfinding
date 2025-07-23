using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class MapData
{
    public string mStartTime = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
    public List<Object> mItems;
    public List<Object_Player> mPlayers;
    public Grid[,] mGrids;
    public int mMapID = -2; //이것으로 다음판으로 넘어갔는지 구별
    public int mMapYsize;
    public int mMapXsize;
    public int mTimeStep = 0;


    public MapData(int[,] pCSVValues)
    {

        mMapID = new System.Random().Next(int.MaxValue);
        mMapYsize = pCSVValues.GetLength(0);
        mMapXsize = pCSVValues.GetLength(1);
        mGrids = new Grid[mMapYsize, mMapXsize]; //csv로부터 만들어낸 int배열 크기를 사용해 그리드 크기 할당 가능

        mPlayers = new List<Object_Player>();
        int mPlayerIndex = 0;
        mItems = new List<Object>();


        for (int i = 0; i < mMapYsize; i++)
        {
            for (int j = 0; j < mMapXsize; j++)
            {

                mGrids[i, j] = new Grid();
                mGrids[i, j].setPosition(j, i, mMapYsize); //초기 위치 잡는다 mGrid[0,0] ~ mGrid[30,30]

                mGrids[i, j].mIsWall = false; //일단 전부 땅으로
                mGrids[i, j].setColor(1.0f, 1.0f, 1.0f); // 일단 전부 흰색으로

                switch (pCSVValues[i, j])
                {
                    case 1: // 벽
                        mGrids[i, j].mIsWall = true;
                        mGrids[i, j].setColor(0.0f, 0.0f, 0.0f);
                        break;

                    case 2: // 플레이어 초기 위치
                        mPlayers.Add(new Object_Player(mPlayerIndex++));
                        mPlayers[mPlayers.Count-1].mObject = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Player2"));
                        mPlayers[mPlayers.Count - 1].mObject.transform.position = new Vector2(((-mMapYsize / 2 + j) * 60), (mMapYsize / 2 - i) * 60); //유니티에선 x와 y를 반대로
                        mPlayers[mPlayers.Count - 1].mNodePositionXY = new Vector2Int(j, i);
                        mPlayers[mPlayers.Count - 1].mFirstPositions = mPlayers[mPlayers.Count - 1].mNodePositionXY;
                        break;

                    case 3: // 아이템 초기 위치
                        mItems.Add(new Object_Item());
                        mItems[mItems.Count-1].mObject = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Item"));
                        mItems[mItems.Count - 1].mObject.transform.position = new Vector2(((-mMapYsize / 2 + j) * 60), (mMapYsize / 2 - i) * 60); //유니티에선 x와 y를 반대로
                        mItems[mItems.Count - 1].mNodePositionXY = new Vector2Int(j, i);
                        break;

                }

            }
        }
    }


    public Vector2Int searchUnityXYToJVectorXY(int x, int y)
    {
        for(int yy = 0; yy < mGrids.GetLength(0); yy++)
        {
            for(int xx = 0; xx < mGrids.GetLength(1); xx++)
            {
                if (mGrids[yy, xx].mObject.transform.position == new Vector3(x, y))
                    return new Vector2Int(xx, yy);
            }
        }
        return new Vector2Int(-1, -1);


    }
}
