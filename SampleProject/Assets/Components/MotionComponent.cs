using UnityEngine;
using System.Collections;

public enum LoopType
{
    OnceHalfWay,
    OnceGoAndBack,
    Forever
}

public abstract class MotionComponent : ComponentBase
{
    // defines how the motion should loop
    public LoopType LoopType { get; protected set; }

    // time to complete the motion, in seconds (loop excluded)
    public float Duration { get; private set; }

    // range of movement of the object, in grid cell unit
    public float Range { get; protected set; }

    // range in world space
    public float RangeWorld
    {
        get
        {
            return Range * scaleFactor;
        }
    }

    public Vector3 InitialPos { get; protected set; }
    public Quaternion InitialRot { get; protected set; }
    public Vector3 InitialScale { get; protected set; }

    private float scaleFactor = 1.0f;
    private float elapsedTime = 0.0f;
    private float motionDirection = 1.0f;

    protected abstract Vector3 ComputePosition(float t);

    protected virtual Quaternion ComputeRotation(float t)
    {
        return InitialRot;
    }

    protected virtual Vector3 ComputeScale(float t)
    {
        return InitialScale;
    }

    public MotionComponent()
    {
        // The constructor is called before values from the level
        // editor are mapped onto the different properties.
        // Consider those as the properties default values in
        // case they are not overriden in the level editor

        Duration = 1.0f;
        Range = 1.0f;
        LoopType = LoopType.Forever;

        scaleFactor = SampleUtils.ComputeScaleFactor();
        motionDirection = 1.0f;
    }

    public override void Initialize()
    {
        InitialPos = transform.position;
        InitialRot = transform.rotation;
        InitialScale = transform.localScale;
    }

    public override void Reset()
    {
        transform.position = InitialPos;
        transform.rotation = InitialRot;
        transform.localScale = InitialScale;

        motionDirection = 1.0f;
        elapsedTime = 0.0f;
    }

    protected void Update()
    {
        // for this sample motion component we rely on the
        // MonoBehavior update function to produce our motion
        elapsedTime = Mathf.Clamp(elapsedTime + Time.deltaTime * motionDirection, 0.0f, Duration);

        // ratio between 0 and 1
        var t = elapsedTime / Duration;

        // position update
        this.transform.position = ComputePosition(t);
        this.transform.rotation = ComputeRotation(t);
        this.transform.localScale = ComputeScale(t);

        // change of direction
        if (elapsedTime >= Duration || elapsedTime <= 0.0f)
        {
            switch (LoopType)
            {
                case LoopType.Forever:
                    motionDirection *= -1.0f;
                    break;
                case LoopType.OnceGoAndBack:
                    if (motionDirection > 0)
                        motionDirection *= -1.0f;
                    break;
                case LoopType.OnceHalfWay:
                    // nothing to do
                    break;
            }
        }
    }
}
