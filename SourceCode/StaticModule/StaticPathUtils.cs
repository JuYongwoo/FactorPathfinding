using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

static public class StaticPathUtils
{
    /*
    static private readonly int[] sDeltaY = new int[] { 1, 0, -1, 0 };
    static private readonly int[] sDeltaX = new int[] { 0, 1, 0, -1 };
    */

    static private readonly int[] sDeltaX = new int[] { 0, 0, -1, 1 };
    static private readonly int[] sDeltaY = new int[] { -1, 1, 0, 0 };

    static public List<Vector2Int> getTrueItems(MapData pMap, bool[,] pTruthes, bool pIsTrue)
    {
        List<Vector2Int> lGoals = new List<Vector2Int>();

        for (int ii = 0; ii < pMap.mItems.Count; ii++)
        {
            if (pIsTrue != pTruthes[pMap.mItems[ii].mNodePositionXY.y, pMap.mItems[ii].mNodePositionXY.x]) // pTruthes 값이 pIsTrue와 다르면 넘어간다.
                continue;

            lGoals.Add(pMap.mItems[ii].mNodePositionXY);
        }

        return lGoals;

    }

    static public List<Vector2Int> getSafeItems(MapData pMap, int[,] pScore, int pSafeDistance)
    {
        List<Vector2Int> lGoals = new List<Vector2Int>();

        for (int ii = 0; ii < pMap.mItems.Count; ii++)
        {
            if (pScore[pMap.mItems[ii].mNodePositionXY.y, pMap.mItems[ii].mNodePositionXY.x] > pSafeDistance)
            {
                lGoals.Add(pMap.mItems[ii].mNodePositionXY);
            }
        }

        return lGoals;

    }

    static public VisionBoundaryBoard getVisionedNodes(MapData pMap, Vector2Int pStartPosition, int maxDistance)
    {
        List<Vector2Int> lVisionedAllNodes = new List<Vector2Int>();
        List<Vector2Int> lBoundaryAllNodes = new List<Vector2Int>();

        for (int d = 0; d <= maxDistance + 1; d++)
        {
            for (int dx = -d; dx <= d; dx++)
            {
                for (int dy = -d; dy <= d; dy++)
                {
                    if (Mathf.Abs(dx) + Mathf.Abs(dy) > d) continue; // Ensure only perimeter of the circle is checked
                    int x = pStartPosition.x + dx;
                    int y = pStartPosition.y + dy;

                    if (x < 0 || x >= pMap.mMapXsize || y < 0 || y >= pMap.mMapYsize)
                        continue;

                    Vector2Int currentNode = new Vector2Int(x, y);

                    if (lVisionedAllNodes.Contains(currentNode) || pMap.mGrids[y, x].mIsWall)
                        continue;

                    if (IsVisioned(pMap, pStartPosition, currentNode))
                    {
                        if (d == maxDistance + 1)
                            lBoundaryAllNodes.Add(currentNode);
                        else
                            lVisionedAllNodes.Add(currentNode);
                    }
                }
            }
        }

        VisionBoundaryBoard lVisionBoundaryBoard = new VisionBoundaryBoard
        {
            mVisionNodeList = lVisionedAllNodes,
            mBoundaryNodeList = lBoundaryAllNodes
        };

        return lVisionBoundaryBoard;
    }




    static public bool IsVisioned(MapData pMap, Vector2Int start, Vector2Int end)
    {
        int x0 = start.x;
        int y0 = start.y;
        int x1 = end.x;
        int y1 = end.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = -Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx + dy;

        while (true)
        {
            if (pMap.mGrids[y0, x0].mIsWall) // Ensure the y-coordinate comes first if needed
            {
                return false;
            }

            if (x0 == x1 && y0 == y1)
            {
                break;
            }

            int e2 = 2 * err;
            if (e2 >= dy) // This comparison was erroneous in the initial version
            {
                if (x0 == x1)
                    break;
                err += dy;
                x0 += sx;
            }
            if (e2 <= dx) // This should check for dx crossing, also corrected comparison direction
            {
                if (y0 == y1)
                    break;
                err += dx;
                y0 += sy;
            }
        }

        return true;
    }
    static public List<Vector2Int> getAllItems(MapData pMap)
    {
        List<Vector2Int> lGoals = new List<Vector2Int>();

        for (int ii = 0; ii < pMap.mItems.Count; ii++)
        {
            lGoals.Add(pMap.mItems[ii].mNodePositionXY);
        }

        return lGoals;

    }

    static public List<Vector2Int> getPathwithBFS(MapData pMap, Vector2Int pStarts, List<Vector2Int> pGoals, int[,] pScoreBoard = null)
    {
        if (pScoreBoard == null) pScoreBoard = new int[pMap.mMapYsize, pMap.mMapXsize];

        List<Vector2Int[]> lParentList = new List<Vector2Int[]>(); // 전체 부모 노드 리스트
        Vector2Int[] lParent = new Vector2Int[pMap.mMapYsize * pMap.mMapXsize];

        Queue<Vector2Int> lQ = new Queue<Vector2Int>();
        lQ.Enqueue(new Vector2Int(pStarts.x, pStarts.y));

        bool[] lIsFound = new bool[pMap.mMapYsize * pMap.mMapXsize];
        lIsFound[pMap.mMapXsize * pStarts.y + pStarts.x] = true;

        int lCount = 0;
        int lFoundGoalCount = 0;

        while (lQ.Count > 0 && lFoundGoalCount < pGoals.Count)
        {
            lCount++;
            Vector2Int lNow = lQ.Dequeue();

            for (int ii = 0; ii < pGoals.Count; ii++)
            {
                if (lNow == pGoals[ii]) //어떤 아이템이든 pGoal에 해당 하는 것을 밟았다면
                {
                    lFoundGoalCount++;
                    lParentList.Add(lParent);
                }
            }

            for (int i = 0; i < 4; i++)
            {
                Vector2Int lNext = new Vector2Int(lNow.x + sDeltaX[i], lNow.y + sDeltaY[i]);

                if (lNext.y < 0 || lNext.y >= pMap.mMapYsize || lNext.x < 0 || lNext.x >= pMap.mMapXsize) continue;
                if (pMap.mGrids[lNext.y, lNext.x].mIsWall) continue;
                int newIndex = pMap.mMapXsize * lNext.y + lNext.x;
                if (lIsFound[newIndex]) continue;

                lIsFound[newIndex] = true;
                lParent[newIndex] = new Vector2Int(lNow.x, lNow.y);
                lQ.Enqueue(lNext);
            }
        }

        List<Vector2Int> lHighestValuePath = getHighestValuePath(pStarts, pGoals, lParentList, pScoreBoard);

        lHighestValuePath.Reverse();
        return lHighestValuePath;
    }


static public List<Vector2Int> getPathwithAstar(MapData pMap, Vector2Int pStarts, List<Vector2Int> pGoals, int[,] pScoreBoard = null)
    {
        if (pScoreBoard == null) pScoreBoard = new int[pMap.mMapYsize, pMap.mMapXsize];

        List<Vector2Int[]> lParentList = new List<Vector2Int[]>(); // 전체 부모 노드 리스트
        Vector2Int[] lParent = new Vector2Int[pMap.mMapYsize * pMap.mMapXsize];
        int[] open = new int[pMap.mMapYsize * pMap.mMapXsize]; // OpenList //효율이 굉장히 좋은 Array.Fill을 사용하기 위해 1차원으로 사용
        bool[] closed = new bool[pMap.mMapYsize * pMap.mMapXsize]; // CloseList

        for (int ii = 0; ii < pGoals.Count; ii++)
        {
            // 각 아이템의 목표 좌표 설정
            Vector2Int lGoal = pGoals[ii];

            // 우선순위 큐 생성 (F 값을 기준으로 정렬)
            PriorityQueue<JVector2IntXY> pq = new PriorityQueue<JVector2IntXY>();
            pq.push(new JVector2IntXY(pStarts.x, pStarts.y, 0, 0)); // 시작점의 G, H는 0

            Array.Fill(open, int.MaxValue);
            Array.Fill(closed, false);


            while (pq.Count > 0)
            {
                JVector2IntXY node = pq.pop();

                // 목표에 도착한 경우 루프 종료
                if (node.mY == lGoal.y && node.mX == lGoal.x)
                {
                    lParentList.Add(lParent);
                    break;
                }

                // 현재 노드를 closed 리스트에 추가

                // 상하좌우로 이동할 수 있는 좌표를 확인
                for (int i = 0; i < sDeltaY.Length; i++)
                {
                    int newX = node.mX + sDeltaX[i];
                    int newY = node.mY + sDeltaY[i];

                    // 지도 범위를 벗어나거나, 벽이거나, 이미 탐색한 경우 건너뛰기
                    if (newX < 0 || newX >= pMap.mMapXsize || newY < 0 || newY >= pMap.mMapYsize) continue;
                    if (pMap.mGrids[newY, newX].mIsWall) continue;
                    int newIndex = pMap.mMapXsize * newY + newX;
                    if (closed[newIndex]) continue;


                    int deltaY = lGoal.y - newY;
                    int deltaX = lGoal.x - newX;
                    int newH = (deltaY < 0 ? -deltaY : deltaY) + (deltaX < 0 ? -deltaX : deltaX);
                    int newG = node.mG + 1; // 이동 비용
                    int newF = newG + newH;


                    // 새로운 F 값이 기존의 값보다 더 크거나 같으면 무시
                    if (open[newIndex] <= newF) continue;

                    // 예약 진행 (OpenList 갱신)
                    closed[newIndex] = true;
                    open[newIndex] = newF;
                    pq.push(new JVector2IntXY() { mH = newH, mG = newG, mY = newY, mX = newX });
                    lParent[newIndex] = new Vector2Int(node.mX, node.mY); // 부모 노드를 기록
                }
            }

        }

        // 간단하게 한번에 가는 경로를 보여주도록 한다 (결과물)
        List<Vector2Int> lHighestValuePath = getHighestValuePath(pStarts, pGoals, lParentList, pScoreBoard);
        lHighestValuePath.Reverse();
        return lHighestValuePath;
    }

    static public List<Vector2Int> getHighestValuePath(Vector2Int pStart, List<Vector2Int> pGoals, List<Vector2Int[]> lParentList, int[,] pScoreBoard)
    {
        List<List<Vector2Int>> lResultPaths = new List<List<Vector2Int>>();

        for (int ii = 0; ii < pGoals.Count; ii++)
        {
            //간단하게 한번에 가는 경로를 보여주도록 한다 (결과물)
            List<Vector2Int> lResultPath = new List<Vector2Int>();
            while (pGoals[ii].y != pStart.y || pGoals[ii].x != pStart.x)
            {
                lResultPath.Add(pGoals[ii]);
                pGoals[ii] = lParentList[ii][pScoreBoard.GetLength(1) * pGoals[ii].y + pGoals[ii].x];
            }
            lResultPaths.Add(lResultPath);
        }


        int lBestIndex = -1;
        int lBestValue = int.MinValue;

        for (int i = 0; i < lResultPaths.Count; i++)
        {
            if(lResultPaths[i].Count <=0) continue; //distance가 0이면 제자리, 제자리 이동은 할 필요가 없음
            int lValue = pScoreBoard[lResultPaths[i][0].y, lResultPaths[i][0].x] - lResultPaths[i].Count;
            if (lValue > lBestValue)
            {
                lBestValue = lValue;
                lBestIndex = i;
            }
        }

        return lResultPaths[lBestIndex];
    }
    static public int[,] getScoreBoardwithBFSfromPlayerCurrentPosition(MapData pMap, int pActorIndex, int[,] pScore)
    {

        pScore[pMap.mPlayers[pActorIndex].mNodePositionXY.y, pMap.mPlayers[pActorIndex].mNodePositionXY.x] = 0; //시작 위치

        // 안전지대 score 여기서 받은 맵으로 각 위치마다 1씩 감소
        Queue<Vector2Int> lQ_Safe = new Queue<Vector2Int>();
        lQ_Safe.Enqueue(pMap.mPlayers[pActorIndex].mFirstPositions); //시작 위치부터 노트 큐 시작
        bool[,] lIsFound = new bool[pMap.mMapYsize, pMap.mMapXsize]; //mIsFound와는 다르다, while문에서 뒤로 탐색하는 것을 방지하기 위해


        //////////여기선 전체 스코어 쫙 1
        while (lQ_Safe.Count > 0)
        {
            Vector2Int lNow = lQ_Safe.Dequeue();

            //parent의 스코어 +1이 현재 노드가 되도록 한다.
            for (int i = 0; i < 4; i++)
            {
                Vector2Int lNext = new Vector2Int(lNow.x + sDeltaX[i], lNow.y + sDeltaY[i]);

                if (lNext.y < 0 || lNext.y >= pMap.mMapYsize || lNext.x < 0 || lNext.x >= pMap.mMapXsize) continue; //경기장 넘어가는 것 방지
                if (pMap.mGrids[lNext.y, lNext.x].mIsWall) continue; //벽은 못간다
                if (lIsFound[lNext.y, lNext.x]) continue; //뒤로 가는 것 방지
                lIsFound[lNext.y, lNext.x] = true;

                pScore[lNext.y, lNext.x] = pScore[lNow.y, lNow.x] - 1; /////////////////// 멀어질수록 10 감소
                lQ_Safe.Enqueue(lNext);
            }

        }
        return pScore;
    }
    static public int[,] getScoreBoardwithBFSfromEnemiesStartPosition(MapData pMap, int pActorIndex)
    {
        int[,] lScore = new int[pMap.mMapYsize, pMap.mMapXsize];
        List<Vector2Int> lEnemiesNowPosition = new List<Vector2Int>();
        // 안전지대 score 여기서 받은 맵으로 각 위치마다 1씩 감소
        Queue<Vector2Int> lQueue = new Queue<Vector2Int>();
        bool[,] lIsFound = new bool[pMap.mMapYsize, pMap.mMapXsize]; //상대들끼리 이미 확인한 곳 공유해야

        for (int i = 0; i < pMap.mPlayers.Count; i++)
        {
            if (i == pActorIndex) continue; //나면 넘어간다, 상대들만.
            lEnemiesNowPosition.Add(pMap.mPlayers[i].mFirstPositions);
        }
        for (int i = 0; i < lEnemiesNowPosition.Count; i++)
        {
            lScore[lEnemiesNowPosition[i].y, lEnemiesNowPosition[i].x] = 0; //시작 위치들은 전부 0으로
            lIsFound[lEnemiesNowPosition[i].y, lEnemiesNowPosition[i].x] = true;
            lQueue.Enqueue(lEnemiesNowPosition[i]); //모든 적들의 시작 위치 큐에 넣어준다.

        }

        int lMax=-1;

        //////////여기선 전체 스코어 쫙 1
        while (lQueue.Count > 0)
        {
            Vector2Int lNow = lQueue.Dequeue();
            lMax = lScore[lNow.y, lNow.x];

            //parent의 스코어 +1이 현재 노드가 되도록 한다.
            for (int i = 0; i < 4; i++)
            {
                Vector2Int lNext = new Vector2Int(lNow.x + sDeltaX[i], lNow.y + sDeltaY[i]);

                if (lNext.y < 0 || lNext.y >= pMap.mMapYsize || lNext.x < 0 || lNext.x >= pMap.mMapXsize) continue; //경기장 넘어가는 것 방지
                if (pMap.mGrids[lNext.y, lNext.x].mIsWall) continue; //벽은 못간다
                if (lIsFound[lNext.y, lNext.x]) continue; //뒤로 가는 것 방지
                lIsFound[lNext.y, lNext.x] = true;

                lScore[lNext.y, lNext.x] = lScore[lNow.y, lNow.x] + 1; /////////////////////////////////////멀어질 수록 10 증가

                lQueue.Enqueue(lNext);

            }

        }

        return lScore;
    }

    static public int[,] substractScoreBoard(int[,] pScoreBoard)
    {
        for (int i = 0; i < pScoreBoard.GetLength(0); i++)
        {
            for (int j = 0; j < pScoreBoard.GetLength(1); j++)
            {
                pScoreBoard[i, j] = pScoreBoard[i, j] - 1;
                if (pScoreBoard[i, j] < 0) pScoreBoard[i, j] = 0;
            }

        }
        return pScoreBoard;
    }


    static public int[,] getScoreBoardwithBFSfromEnemiesCurrentPosition(MapData pMap, int pActorIndex)
    {
        int[,] lScore = new int[pMap.mMapYsize, pMap.mMapXsize];
        List<Vector2Int> lEnemiesNowPosition = new List<Vector2Int>();
        // 안전지대 score 여기서 받은 맵으로 각 위치마다 1씩 감소
        Queue<Vector2Int> lQueue = new Queue<Vector2Int>();
        bool[,] lIsFound = new bool[pMap.mMapYsize, pMap.mMapXsize]; //상대들끼리 이미 확인한 곳 공유해야

        for (int i = 0; i < pMap.mPlayers.Count; i++)
        {
            if (i == pActorIndex) continue; //나면 넘어간다, 상대들만.
            lEnemiesNowPosition.Add(pMap.mPlayers[i].mNodePositionXY);
        }
        for (int i = 0; i < lEnemiesNowPosition.Count; i++)
        {
            lScore[lEnemiesNowPosition[i].y, lEnemiesNowPosition[i].x] = 0; //시작 위치들은 전부 0으로
            lIsFound[lEnemiesNowPosition[i].y, lEnemiesNowPosition[i].x] = true;
            lQueue.Enqueue(lEnemiesNowPosition[i]); //모든 적들의 시작 위치 큐에 넣어준다.

        }


            //////////여기선 전체 스코어 쫙 1
            while (lQueue.Count > 0)
        {
            Vector2Int lNow = lQueue.Dequeue();

            //parent의 스코어 +1이 현재 노드가 되도록 한다.
            for (int i = 0; i < 4; i++)
            {
                Vector2Int lNext = new Vector2Int(lNow.x + sDeltaX[i], lNow.y + sDeltaY[i]);

                if (lNext.y < 0 || lNext.y >= pMap.mMapYsize || lNext.x < 0 || lNext.x >= pMap.mMapXsize) continue; //경기장 넘어가는 것 방지
                if (pMap.mGrids[lNext.y, lNext.x].mIsWall) continue; //벽은 못간다
                if (lIsFound[lNext.y, lNext.x]) continue; //뒤로 가는 것 방지
                lIsFound[lNext.y, lNext.x] = true;

                lScore[lNext.y, lNext.x] = lScore[lNow.y, lNow.x] + 1; /////////////////////////////////////멀어질 수록 10 증가

                lQueue.Enqueue(lNext);

            }

        }
        return lScore;
    }


}
