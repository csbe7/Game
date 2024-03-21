using Godot;
using System;

public partial class Uppercut : GenericAbility
{
    
    [Export] private float damage;
    [Export] private float hitstunDuration;
    [Export] private float knockbackStrenght;
    [Export] private float knockbackY;

    [Export] private float hitboxOffset;
   

    public override void Selection(Godot.Collections.Dictionary dict)
    {
        if (confirm)
        {
            direction = (Vector3)dict["position"]; 
            direction = new Vector3(direction.X, 0f, direction.Z);
            SelectionDone = true;
            GD.Print("SelectionDone");
        }
    }


    public override void TriggerPull()
    {
        hitboxInstance = LoadHitbox(hitboxShape);
        User.c.AddChild(hitboxInstance);
        
        var Pos = User.GlobalPosition -User.GlobalBasis.Z * hitboxOffset;
        Pos.Y += 2;
        hitboxInstance.GlobalPosition = Pos;

        hitboxInstance.BodyEntered += OnHitboxDetection;

    }

    public override void TriggerRelease()
    {   
        ClearHitboxes();
    }


    public override void Use()
    {
        User.c.state = Controller.State.busy;
        User.c.LookAt(direction);

        User.c.target = User.c.GlobalPosition;
        User.c.Velocity = Vector3.Zero;
        
        SetAnimation(name);

        transition = User.c.TransitionTo("Ability", 0.1f, true);
        
    }

    public override void End()
    {
        ClearHitboxes();
        QueueFree();
        
        if (IsInstanceValid(transition)) 
        {
            
        }

        User.c.state = Controller.State.free;
        User.c.stateMachine.Travel("Idle");
    }


    public void OnHitboxDetection(Node3D body)
    {
        if(body == (Node3D)(User.GetParent())) return;

        var sheet = body.GetNode<CharacterSheet>("CharacterSheet");
        
        if (sheet == null) return;

        Vector3 knockback = (body.GlobalPosition - User.c.GlobalPosition).Normalized() * knockbackStrenght;
        knockback.Y += knockbackY;
        AttackInfo attackInfo = new AttackInfo(damage, knockback, hitstunDuration, User);
        sheet.TakeDamage(attackInfo);
    }
}
