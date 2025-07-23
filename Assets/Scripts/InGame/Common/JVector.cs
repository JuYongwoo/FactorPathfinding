using System;

public struct JVector2IntXY : IComparable<JVector2IntXY>
{
    public int mX;
    public int mY;
    public int mH;
    public int mG;

    public JVector2IntXY(int pX, int pY, int pH = 0, int pG = 0)
    {
        mX = pX;
        mY = pY;
        mH = pH;
        mG = pG;
    }

    public int CompareTo(JVector2IntXY other)
    {
        int thisF = mH + mG;
        int otherF = other.mH + other.mG;

        if (thisF == otherF)
            return 0;
        return thisF < otherF ? 1 : -1;
    }

}
