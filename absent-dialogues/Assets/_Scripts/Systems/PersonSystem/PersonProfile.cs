using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Person Profile", fileName = "New Person Profile")]
public class PersonProfile : ScriptableObject
{
    public string Name;
    [Multiline(10)] public string Backstory;
    public Sprite Icon;
}
