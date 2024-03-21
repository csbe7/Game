using Godot;
using System;

public partial class Root : Node
{
    public override void _Ready()
    {
        GetTree().CallGroup("FinalReady", "_FinalReady");
    }
}
