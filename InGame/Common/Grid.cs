using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Grid :IComparable<Grid>
{

    public int mF;
    public int mG;
    public JVector2IntXY mNodePositionXY;


    public bool mIsWall = false;
    public GameObject mObject;
   
    public int mFactorScore = 0;



    public Grid()
    {
        mObject = UnityEngine.GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Ground"));
    }
    public void setPosition(int pX, int pY, int pMapSize)
    {
        mNodePositionXY = new JVector2IntXY() { mX = pX, mY = pY };
        mObject.transform.position = new Vector2( ((-pMapSize / 2 + pX) * 60), (pMapSize / 2 - pY) * 60); //유니티에선 x와 y를 반대로
    }


    public void setColor(float r, float g, float b) //벽인지 땅인지 바꾼다
    {
        mObject.GetComponent<SpriteRenderer>().color = new Color(r, g, b);
    }

    public int CompareTo(Grid other)
    {
        if(mF == other.mF) return 0; // 같으면 0 반환
        else if (mF < other.mF) return 1; //작으면 1
        else return -1; //크면 -1
        
    }
}
