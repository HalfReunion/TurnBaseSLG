using UnityEngine;

[CreateAssetMenu(fileName = "BuffData", menuName = "Buff/BuffData")]
public class BuffData : ScriptableObject {
    public string BuffName;
    public BuffType buffType;
    public UnitType unitType;
    public int lastTurn;
    public int value;
}
