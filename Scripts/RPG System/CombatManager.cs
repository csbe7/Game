using Godot;
using System;
using System.Reflection.Metadata;

public partial class CombatManager : Node
{
    private GameManager gm;
    private CombatUI UI;

    private CameraControls cam;
	public Godot.Collections.Array<CharacterSheet> party = new Godot.Collections.Array<CharacterSheet>{};
    public CharacterSheet selectedCharacter;

    public override void _Ready()
    {
        gm = GetNode<GameManager>("%GameManager");
        UI = GetNode("%UI").GetNode<CombatUI>("CombatUI");

        cam = GetNode<CameraControls>("%CameraPivot");

        var nodes = GetTree().GetNodesInGroup("Player Character"); //GET PLAYER CHARACTERS
        foreach(Node n in nodes)
		{
            party.Add(n.GetNode<CharacterSheet>("CharacterSheet"));
		}
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("Debug")) //DEBUG
        {
            foreach(CharacterSheet sheet in party)
            {
                GD.Print(sheet.c.state);
            }
        }

        TimeStop();
        if (Engine.TimeScale == 0)
        {
            Select();
            if (Input.IsActionJustPressed("Select"))
            {
                Fire();
            }
        }
        
    }


    public void TimeStop()
    {
        bool timeStop = false;
        foreach(CharacterSheet character in party)
        {
            if (character.c.state == Controller.State.free)
            {
                timeStop = true;
                break;
            } 
        }
        if (timeStop) Engine.TimeScale  = 0;
        else Engine.TimeScale = 1;
    } 
    
    public enum SelectState{
        character_select,
        ability_select,
        target_select,
    }
    public SelectState selectState = SelectState.character_select;

    void Select()
    {
        switch (selectState)
        {
            case SelectState.character_select:
            CharacterSelect();
            break;

            case SelectState.ability_select:
            CharacterSelect();
            break;

            case SelectState.target_select:
            if (selectedCharacter.abilityInstance.SelectionDone) CharacterSelect();
            if (selectState == SelectState.target_select) TargetSelect();
            break;
        }
    }

    void CharacterSelect()
    {
        Godot.Collections.Dictionary raycast = null;
        if (Input.IsActionJustPressed("LeftClick"))raycast = cam.ShootRay(1);
        if (raycast != null)
        {
            var body = (PhysicsBody3D)raycast["collider"];
            
			if (body != null && body.IsInGroup("Player Character"))
			{
                selectedCharacter = body.GetNode<CharacterSheet>("CharacterSheet");
                
                if (IsInstanceValid(selectedCharacter) && IsInstanceValid(selectedCharacter.abilityInstance)) selectState = SelectState.target_select;
                else if (IsInstanceValid(selectedCharacter)) selectState = SelectState.ability_select;
                UI.UpdateUI();
            }
        }
    }
    public void AbilitySelect(int index)
    {
        if (selectState != SelectState.ability_select) return;

        selectedCharacter.selectedAbility = index;
        selectedCharacter.InstanceAbility();
        selectState = SelectState.target_select;
        UI.UpdateUI();
    }
    void TargetSelect()
    {
        if (selectedCharacter.c.state != Controller.State.free)
        {
            selectState = SelectState.character_select;
            return;
        } 
        
        /*if (IsInstanceValid(selectedCharacter.abilityInstance))
        {
            GD.Print("Error");
        }*/

        if (Input.IsActionJustPressed("TestButton2")) //Go to previous state
        {
            if (IsInstanceValid(selectedCharacter.abilityInstance)) selectedCharacter.abilityInstance.QueueFree();
            selectState = SelectState.ability_select;
            return;
        }

        Godot.Collections.Dictionary raycast = cam.ShootRay(1);
        if (raycast != null)
        {
           if (!selectedCharacter.abilityInstance.SelectionDone) selectedCharacter.abilityInstance.Selection(raycast);
        }
    }

    void Fire()
    {
        foreach(CharacterSheet ch in party)
        {
            if (IsInstanceValid(ch.abilityInstance) && ch.abilityInstance.SelectionDone)
            {
                //ch.SetState(Controller.State.busy);
                ch.UseAbility();
            } 
        }
        selectState = SelectState.character_select;
        UI.UpdateUI();
    }

}
