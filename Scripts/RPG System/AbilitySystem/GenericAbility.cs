using Godot;
using System;

[GlobalClass]
public partial class GenericAbility : Node
{
    [Export] public string name;
    [Export] public string cost;
    //[Export] public AttackInfo attackInfo;
    [Export] public Shape3D hitboxShape;
    
    public CharacterSheet User;
    public Area3D hitboxInstance;

    public Vector3 direction;
    public bool SelectionDone = false;
    public bool confirm = false;

    public AnimationNodeStateMachineTransition transition = null;

    public Area3D LoadHitbox(Shape3D shape)
    {
        Area3D instance;
        var Hitbox = GD.Load<PackedScene>("res://Prefabs/hitbox.tscn");
        instance = Hitbox.Instantiate<Area3D>();

        instance.Monitorable = false;
        instance.Monitoring = true;

        instance.GetNode<CollisionShape3D>("CollisionShape3D").Shape = shape;
        GD.Print(instance);
        
        return instance;
    }

    public void SetAnimation(string animName)
    {
        AnimationNodeAnimation animNode = (AnimationNodeAnimation)User.c.animationStateMachine.GetNode("Ability");
        animNode.Animation = (User.animLibrary+"/"+animName);
    }
    
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventKey)
        {
            if (eventKey.Pressed && eventKey.IsAction("LeftClick") && !eventKey.IsEcho())
            {
                confirm = true;
            }
        }
    }

    public virtual void ClearHitboxes(){
        if (IsInstanceValid(hitboxInstance)) hitboxInstance.QueueFree();
    }

    public virtual void TriggerPull(){}
    public virtual void TriggerRelease(){}
    public virtual void Use(){}

    public virtual void Selection(Godot.Collections.Dictionary dict){}
    public virtual void Interrupt(AttackInfo attackInfo){
        ClearHitboxes();
        QueueFree();
    }
    public virtual void End(){
        ClearHitboxes();
        QueueFree();
        User.c.state = Controller.State.free;
    }
}
