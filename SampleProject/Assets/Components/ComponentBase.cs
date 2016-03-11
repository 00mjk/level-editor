using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract class ComponentBase : MonoBehaviour
{
    // This base class is rather empty in this sample and could pretty
    // much just be an interface, but in a larger project it would contain
    // all the basic shared feature of your components.
    // 
    // For example in Jelly Arcade this is where components would keep references
    // to other important objects of the game (eg. player, world boundaries...),
    // this is also where we managed inter-components relationships so that
    // different block elements could be aware of each other (via a 'sibling'
    // component) and then create more advanced configuations.

    // NameSuffix is added at the end of the gameObject name at creation
    // time to easily find a specific component in the object hierarchy
    public abstract string NameSuffix { get; }

    // we decided to move the component initialization to a dedicated function
    // to be in control of the timing. Keep in mind that components might need
    // information from various elements of your game, and even from other
    // components attached to other block elements if they are designed to work
    // together. Then being in control of the initialization timing becomes essential.
    public abstract void Initialize();

    public abstract void Reset();
}
