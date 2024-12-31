using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;


public struct VisionBoundaryBoard
{
    public List<Vector2Int> mVisionNodeList;
    public List<Vector2Int> mBoundaryNodeList;
}

public struct ScoreDangerBoard
{
    public int[,] mScoreBoard;
    public bool[,] mDangerBoard;
}
public enum PathFinding : int //현재 어떤 길찾기 알고리즘이 실행 중인가?
{
    None = 0, //실행 중이지 않을 때
    RightHand,
    Dijikstra,
    BFS,
    FactorAStar,
    AStar,
}

public enum GameState : int //현재 어떤 길찾기 알고리즘이 실행 중인가?
{
    MainMenu,
    Stage
}

public enum Direction : int
    {
        Up= 0,
        Left = 1,
        Down= 2,
        Right = 3
    }
    public enum DirectionForward : int //현재 플레이어가 보고있는 방향(이전 그리드에서 다음 그리드 차이) //Board에서 사용된다
    {
        Forward= 0,
        Left = 1,
        Backward= 2,
        Right = 3
    }

    public enum MapType : int
    {
        SideWinder,
        Map01
    }


