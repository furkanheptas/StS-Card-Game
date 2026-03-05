using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "Game/Player Data")]
public class PlayerData : ScriptableObject
{
    public string playerName = "Player";
    public Sprite sprite;
    public int maxHP = 80;
}
