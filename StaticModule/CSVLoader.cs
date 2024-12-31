using System.Collections.Generic;
using System.IO;
using UnityEngine;

static public class CSVLoader
{
    static public int[,] LoadGrid(string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);

        // 첫 번째 줄을 기준으로 배열의 열 크기를 결정
        string[] firstLine = lines[0].Trim().Split(',');
        int rows = lines.Length;
        int cols = firstLine.Length;

        // Grid 배열 초기화
        int[,] gridvalue = new int[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            string[] row = lines[i].Trim().Split(',');
            for (int j = 0; j < cols; j++)
            {

                if (!string.IsNullOrEmpty(row[j].Trim())) // 빈 문자열 검사 추가
                {
                    try
                    {
                        int value = int.Parse(row[j].Trim()); // 데이터 정리 및 파싱
                        gridvalue[i, j] = value;
                    }

                    catch (System.FormatException e)
                    {
                        Debug.LogError($"FormatException at line {i}, column {j}: '{row[j]}' could not be parsed as an integer. {e}");
                    }
                }
            }
        }
        return gridvalue;
    }

}
