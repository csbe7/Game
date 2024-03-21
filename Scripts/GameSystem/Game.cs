using Godot;
using System;



public partial class Game : Node
{
    public enum GameState
    {
        menu,
        gameplay_explore,
        gameplay_combat,
    }

    public GameState state = GameState.gameplay_combat;
    public bool hoveringButton = false;

    //public float scaledTime = 1;
}
