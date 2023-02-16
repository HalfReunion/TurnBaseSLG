using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="ShootAnima",menuName = "AnimaInfo/ShootAnima")]
public class ShootAnimaInfo : ScriptableObject
{
    public AnimationClip clip;
    public List<int> keys;
    public Transform shootPoint;
}
