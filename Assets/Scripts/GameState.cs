using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameState", menuName = "PathOfPixel/GameState", order = 1)]
public class GameState : ScriptableObject
{
    public PlayerState playerState;

    public GameState()
    {
        playerState = new PlayerState();
    }
}
