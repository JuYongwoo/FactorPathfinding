using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

public class MapGenerator
{
    protected int mMapYSize;
    protected int mMapXSize;

    public int[,] makeNewMap(int pMapSize)
    {

        mMapYSize = pMapSize;
        int [,]mMap = new int[pMapSize, pMapSize];

        int lItemsCount = 0;
        System.Random random = new System.Random();


        // 플레이어와 적의 위치를 제외한 모든 가능한 좌표를 리스트로 저장합니다.
        List<JVector2IntXY> possiblePositions = new List<JVector2IntXY>();
        for (int y = 0; y < pMapSize; y++)
        {
            for (int x = 0; x < pMapSize; x++)
            {
                if (mMap[y, x] == 0) possiblePositions.Add(new JVector2IntXY() { mX = x, mY = y }); //Ground, 0이면 아이템을 둘 수 있다.
            }
        }

        // 리스트를 랜덤하게 섞습니다.
        possiblePositions = possiblePositions.OrderBy(pos => random.Next()).ToList();

        // 맵에 아이템을 배치합니다.
        foreach (var pos in possiblePositions)
        {
            if (lItemsCount >= StaticVariables.sItemMaxCount) break;

            mMap[pos.mY, pos.mX] = 3;
            lItemsCount++;
        }

        mMap[1, 1] = 2;
        mMap[pMapSize-2, pMapSize - 2] = 2;

        /*
        // 만약 아이템이 하나도 놓이지 않았다면 다시 맵을 생성합니다.
        if (lItemsCount == 0)
        {
            makeNewMap(pMapSize);
        }
        */
        SaveMazeToFile(mMap);
        return mMap;
    }

    public int[,] makeItems(int[,] pMap)
    {
        mMapYSize = pMap.GetLength(0);
        mMapXSize = pMap.GetLength(1);


        int[,] mMap = new int[mMapYSize, mMapXSize] ;
        for (int i = 0; i < mMapYSize; i++)
        {
            for (int j = 0; j < mMapXSize; j++)
            {
                mMap[i,j] = pMap[i, j];
            }
        }




        int lItemsCount = 0;
        System.Random random = new System.Random();


        // 플레이어와 적의 위치를 제외한 모든 가능한 좌표를 리스트로 저장합니다.
        List<JVector2IntXY> possiblePositions = new List<JVector2IntXY>();
        for (int y = 0; y < mMapYSize; y++)
        {
            for (int x = 0; x < mMapXSize; x++)
            {
                if(mMap[y, x] == 0) possiblePositions.Add(new JVector2IntXY() { mX = x, mY = y }); //Ground, 0이면 아이템을 둘 수 있다.
            }
        }

        // 리스트를 랜덤하게 섞습니다.
        possiblePositions = possiblePositions.OrderBy(pos => random.Next()).ToList();

        // 맵에 아이템을 배치합니다.
        foreach (var pos in possiblePositions)
        {
            if (lItemsCount >= StaticVariables.sItemMaxCount) break;

            mMap[pos.mY, pos.mX] = 3;
            lItemsCount++;
        }

        /*
        // 만약 아이템이 하나도 놓이지 않았다면 다시 맵을 생성합니다.
        if (lItemsCount == 0)
        {
            makeItems(pMap);
        }
        */
        SaveMazeToFile(mMap);
        return mMap;
    }
    protected string SaveMazeToFile(int[,] pMap)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < mMapYSize; i++)
        {
            for (int j = 0; j < mMapXSize; j++)
            {
                sb.Append(pMap[i, j]);
                if (j < mMapXSize - 1) sb.Append(",");
            }
            sb.AppendLine();
        }

        string directoryPath = Application.streamingAssetsPath + "/Maps";
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        int randomNumber = new System.Random().Next(1000);
        string filePath = Path.Combine(directoryPath, $"RandomFPSMap{randomNumber}.csv");
        File.WriteAllText(filePath, sb.ToString()); //경로에 파일 저장
        //Debug.Log($"Maze saved to {filePath}");

        return $"RandomFPSMap{randomNumber}.csv"; // 파일 이름을 반환한다.
    }

}
