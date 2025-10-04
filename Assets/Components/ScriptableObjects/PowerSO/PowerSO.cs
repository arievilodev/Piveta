using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Power", menuName = "Scriptable Objects/Power")]
public class PowerSO : ScriptableObject
{
    public int id;
    public string name;
    public string description;
    public Sprite icon;
    public int duration;
    public int cooldown;
}
