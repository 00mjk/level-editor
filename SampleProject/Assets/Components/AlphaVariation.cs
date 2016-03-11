using UnityEngine;
using System.Collections;

public class AlphaVariation : ComponentBase
{
    public override string NameSuffix
    {
        get { return "alpha"; }
    }

    // transparency reached when getting invisible, between 0 and 1 included
    public float MinAlpha { get; protected set; }

    // transparency reached when getting visible, between 0 and 1 included
    public float MaxAlpha { get; protected set; }

    // duration to fade in/out between extrema, in seconds
    public float FadeDuration { get; private set; }

    // duration the object stays at the extrema, in seconds
    public float ExtremumDuration { get; private set; }

    private MeshRenderer meshRenderer;
    private Coroutine coroutine;

    public AlphaVariation()
    {
        // The constructor is called before values from the level
        // editor are mapped onto the different properties.
        // Consider those as the properties default values in
        // case they are not overriden in the level editor

        MinAlpha = 0.0f;
        MaxAlpha = 1.0f;
        FadeDuration = 1.0f;
        ExtremumDuration = 1.0f;
    }

    public override void Initialize()
    {
        // properties values sanity check
        MinAlpha = Mathf.Clamp01(MinAlpha);
        MaxAlpha = Mathf.Clamp01(MaxAlpha);

        // retrieve the renderer
        meshRenderer = this.GetComponent<MeshRenderer>();
        Debug.Assert(meshRenderer != null, "No sprite renderer was found");

        Reset();

        coroutine = StartCoroutine(VariationCoroutine());
    }

    public override void Reset()
    {
        // start coroutine
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    protected void OnEnable()
    {
        if (coroutine == null)
            Initialize();
    }

    protected void OnDisable()
    {
        Reset();
    }

    private IEnumerator VariationCoroutine()
    {
        // for this component we run the logic in a coroutine
        var elapsedTime = 0.0f;
        while (true)
        {
            while (elapsedTime < FadeDuration)
            {
                elapsedTime = Mathf.Min(FadeDuration, elapsedTime + Time.deltaTime);
                SetAlphaValue(Mathf.Lerp(MinAlpha, MaxAlpha, elapsedTime / FadeDuration));
                yield return null;
            }

            yield return new WaitForSeconds(ExtremumDuration);

            while (elapsedTime > 0)
            {
                elapsedTime = Mathf.Max(0.0f, elapsedTime - Time.deltaTime);
                SetAlphaValue(Mathf.Lerp(MinAlpha, MaxAlpha, elapsedTime / FadeDuration));
                yield return null;
            }

            yield return new WaitForSeconds(ExtremumDuration);
        }
    }

    private void SetAlphaValue(float alpha)
    {
        var color = meshRenderer.material.color;
        color.a = alpha;
        meshRenderer.material.color = color;
    }
}
