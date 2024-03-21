using Godot;
using System;

public partial class CombatUI : Node
{
    private GameManager gm;
    private CombatManager cm;

    private Control abilitySelect;
    GridContainer AbilityGrid;

    private Control targetSelect;


    public override void _Ready()
    {
        /*gm = GetNode<GameManager>("%GameManager");
        cm = GetNode<CombatManager>("%CombatManager");*/

        abilitySelect = GetNode<Control>("AbilitySelect");
        AbilityGrid = GetNode<GridContainer>("AbilitySelect/GridContainer");

        targetSelect = GetNode<Control>("TargetSelect");

        
    }
    public void _FinalReady()
    {
        gm = GetNode<GameManager>("%GameManager");
        cm = GetNode<CombatManager>("%CombatManager");

        UpdateUI();
    }

    public void UpdateUI()
    {
        switch (cm.selectState)
        {
            case CombatManager.SelectState.character_select:
            abilitySelect.Hide();
            targetSelect.Hide();
            break;

            case CombatManager.SelectState.ability_select:
            AbilitySelectUI();
            break;

            case CombatManager.SelectState.target_select:
            TargetSelectUI();
            break;
        }
       
    }

    void AbilitySelectUI()
    {
        abilitySelect.Show();
        targetSelect.Hide();
        while (AbilityGrid.GetChildCount() != 0)
        {
            var child = AbilityGrid.GetChild(0);
            var button = child.GetNode<AbilityButton>("Inside/Button");
            button.AbilitySelected -= cm.AbilitySelect;

            AbilityGrid.RemoveChild(child);
            child.QueueFree();
        }
        
        var slot = GD.Load<PackedScene>("res://UI/CombatUI/AbiltySlot.tscn");
        int i = 0;
        foreach (string a in cm.selectedCharacter.abilities)
        {
            var instance = slot.Instantiate<Control>();
            instance.GetNode<Label>("Inside/AbilityName").Text = a;
            AbilityGrid.AddChild(instance);

            var button = instance.GetNode<AbilityButton>("Inside/Button");
            button.AbilitySelected += cm.AbilitySelect;
            button.AbilityIndex = i;
            i++;
        }
        
        
    }
    void TargetSelectUI()
    {
        abilitySelect.Hide();
        targetSelect.Show();

        var name = targetSelect.GetNode<Label>("AbilitySlot/Inside/AbilityName");
        name.Text = cm.selectedCharacter.abilityInstance.name;
    }
}
