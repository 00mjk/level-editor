using UnityEngine;
using System.Collections;

public class HorizontalMotion : MotionComponent
{
    public override string NameSuffix
    {
        get { return "horizontal"; }
    }

    protected Vector3 endPos;

    public override void Initialize()
    {
        base.Initialize();
        endPos = new Vector3(InitialPos.x + RangeWorld, InitialPos.y, InitialPos.z);
    }

    protected override Vector3 ComputePosition (float t)
    {
        return Vector3.Lerp(InitialPos, endPos, t);
    }
}
