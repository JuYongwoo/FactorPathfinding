using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

static public class StaticVariables
{
    static public string sTag = "Default";
    static public int sItemMaxCount = 5;
    static public int sMapSize = 49;
    static public bool sIsMapStick = false;
    static public int sGameOverDistance = 20; //상대로부터 어디까지 danger zone? & 닿으면 게임 오버(이건 성능 상 직사각형으로)
    static public int sVisionDistance = 10; //Vision 들어간 것만 적용

    static public int sReGameCount = 0; //수 세는 용
    static public readonly int sReGameMaxCount = 500; //500이 되면 게임이 꺼지도록
}
