using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Person", fileName = "New Person")]
public class Person : ScriptableObject
{
    public string Name;
    public string Description;
    public Sprite Icon;
}
