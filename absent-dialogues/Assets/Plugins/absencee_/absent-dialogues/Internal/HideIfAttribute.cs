using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class HideIfAttribute : PropertyAttribute
{
    public string PropertyName { get; private set; }
    public object TargetValue { get; private set; }
    public bool DirectBool { get; private set; }

    public HideIfAttribute(string comparedPropertyName)
    {
        this.PropertyName = comparedPropertyName;

        DirectBool = true;
    }

    public HideIfAttribute(string comparedPropertyName, object targetValue)
    {
        this.PropertyName = comparedPropertyName;
        this.TargetValue = targetValue;

        DirectBool = false;
    }
}