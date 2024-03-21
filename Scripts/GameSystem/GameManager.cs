using Godot;
using System;

public partial class GameManager : Node
{
    //DEBUG DEBUG DEBUG DEBUG DEBUG
    public Game game;

    private CombatManager cm;
    private PartyManager pm;

    public enum Mode{
        explore,
        combat,
    }

    private Mode mode;

    public override void _Ready()
    {
        game = GetNode<Game>("/root/Game");

        cm = GetNode<CombatManager>("%CombatManager");
        pm = GetNode<PartyManager>("%PartyManager");

        cm.SetProcess(false);
        Engine.TimeScale = 1;

        mode = Mode.combat;
    }

    public override void _Process(double delta)
    {
        if (mode == Mode.combat) Combat();
    }

    void Combat()
    {
        if (Input.IsActionJustPressed("Select"))
        {
            cm.SetProcess(true);
            pm.SetProcess(false);
        }  
        else if (Input.IsActionJustPressed("Cancel"))
        {
            cm.SetProcess(false);
            pm.SetProcess(true);
            Engine.TimeScale = 1;
        }
    }
}
