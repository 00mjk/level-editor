using UnityEngine;
using System.Collections;

public class VerticalMotion : MotionComponent
{
    public override string NameSuffix
    {
        get { return "vertical"; }
    }

    protected Vector3 endPos;

    public override void Initialize()
    {
        base.Initialize();
        endPos = new Vector3(InitialPos.x, InitialPos.y + RangeWorld, InitialPos.z);
    }

    protected override Vector3 ComputePosition(float t)
    {
        return Vector3.Lerp(InitialPos, endPos, t);
    }
}