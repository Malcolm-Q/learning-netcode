using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/levelDescription", order = 1)]
public class levelDescription : ScriptableObject
{
    public int sceneNumber;
    public string levelName;
    public Sprite thumbnail;
}
