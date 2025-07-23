using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


static public class RecordSaver
{
    static string directoryPath = Path.Combine(Application.dataPath, StaticVariables.sTag +"." + StaticVariables.sMapSize + "." + StaticVariables.sItemMaxCount + "." + StaticVariables.sGameOverDistance);
    static public void WriteToCSV(List<long> pValue, string pFileName)
    {

        // 폴더가 존재하지 않으면 생성
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        try
        {
            // CSV 파일에 기록
            using (StreamWriter writer = new StreamWriter(Path.Combine(directoryPath, pFileName + ".csv"), true))
            {
                for(int i = 0; i < pValue.Count; i++)
                {
                    string csvLine = pValue[i].ToString();
                    writer.WriteLine(csvLine);

                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error writing to CSV: " + ex.Message);
        }
    }

    static public void WriteToCSV(long pValue, string pFileName)
    {

        // 폴더가 존재하지 않으면 생성
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        try
        {
            // CSV 파일에 기록
            using (StreamWriter writer = new StreamWriter(Path.Combine(directoryPath, pFileName + ".csv"), true))
            {
            string csvLine = pValue.ToString();
            writer.WriteLine(csvLine);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error writing to CSV: " + ex.Message);
        }
    }
}
