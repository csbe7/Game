using Godot;
using System;
using System.Data;
using System.Runtime.ExceptionServices;

public partial class CharacterSheet : Node3D
{
	public AnimationPlayer ap;
	public AnimationTree at;
	[Export] public string animLibrary;
	[Export] public string overrideLibrary;
	public Controller c;

    public override void _Ready()
    {
        ap = GetNode<AnimationPlayer>("../AnimationPlayer");
		at = GetNode<AnimationTree>("../AnimationTree");
		c = GetNode<Controller>("..");
    }

    [Export]Godot.Collections.Dictionary stats = new Godot.Collections.Dictionary
	{
		//STATUS STATS
		{"MaxHealth", new Stat(100)},
        {"CurrentHealth", new Stat(100)},
		{"MaxStamina", new Stat(100)},
		{"CurrentStamina", new Stat (100)},

        //MAIN STATS
		{"Strenght", new Stat(1)},
		{"Agility", new Stat(1)},
		{"Resistance", new Stat(1)},

		//SUB STATS
		{"Damage", new Stat(0)},
		{"Force", new Stat(0)},
		{"Speed", new Stat(0)},
		{"DamageReduction", new Stat(0)},
		{"ForceResist", new Stat(0)}
	};
    
	[Export] public Godot.Collections.Array<string> abilities = new Godot.Collections.Array<string>();
	public int selectedAbility = -1;
	public GenericAbility abilityInstance;
    

    
	public Timer t;
    public void TakeDamage(AttackInfo attackInfo)
	{
		if (attackInfo.hitstunDuration > 0f) //HITSTUN
		{
			if (IsInstanceValid(abilityInstance)) abilityInstance.Interrupt(attackInfo);

            c.state = Controller.State.hitstun;

			t = new Timer();
		    AddChild(t);
			t.Start(attackInfo.hitstunDuration);
			t.Timeout += EndHitstun; 
			t.Timeout += t.QueueFree;
			c.stateMachine.Start("Hitstun");

			c.LookAt(-attackInfo.direction * 1000);
		} 

		var hp =  ((Stat)stats["CurrentHealth"]).baseValue; //HP
		hp = Mathf.Clamp(hp-attackInfo.damage, 0f, ((Stat)stats["MaxHealth"]).ModValue);
		((Stat)stats["CurrentHealth"]).baseValue = hp;

		GD.Print(((Stat)stats["CurrentHealth"]).baseValue);
		c.Velocity = attackInfo.knockback;

	}


	public void TriggerPull()
	{
		abilityInstance.TriggerPull();
	} 
	public void TriggerRelease()
	{
		abilityInstance.TriggerRelease();
	}
    public void UseAbility()
	{
		abilityInstance.Use();
	}
	public void EndAbility()
	{
		abilityInstance.End();
	}


	public void SetState(Controller.State s)
	{
		
	}
	public void EndHitstun()
	{
		c.Velocity = Vector3.Zero;
		c.target = c.GlobalPosition;
		c.stateMachine.Travel("Idle");
		c.state = Controller.State.free;

	}

	public void InstanceAbility()
	{
        //RemoveAbility();

		abilityInstance = (GenericAbility)GD.Load<PackedScene>("res://Scripts/RPG System/AbilitySystem/Abilities/"+abilities[selectedAbility]+".tscn").Instantiate();
	    AddChild(abilityInstance);
		abilityInstance.User = this;
	}

	
}
