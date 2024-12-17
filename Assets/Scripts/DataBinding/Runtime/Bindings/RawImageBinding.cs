using System;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RawImage))]
public class RawImageBinding : DataBinding
{
    public override Type BindingType => typeof(RawImage);

    protected override void Setup()
    {
        FindTarget<RawImage>();
    }
}
