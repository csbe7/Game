using Godot;
using Godot.Collections;
using System;


public partial class Move : GenericAbility
{
    
     
    public override void Selection(Dictionary dict)
    {
        if (confirm)
        {
            direction = (Vector3)dict["position"];
            SelectionDone = true;
        }
    }
    
    public override void Use()
    {
        User.c.state = Controller.State.busy;

        User.c.nav.TargetPosition = direction;
     
        User.c.LookAt(direction * 100);
        
        BoxShape3D characterHitbox = (BoxShape3D)User.c.GetNode<CollisionShape3D>("CollisionShape3D").Shape; 
        

        hitboxInstance = LoadHitbox(characterHitbox);
        GD.Print("real" + hitboxInstance);
        User.c.AddChild(hitboxInstance);
        
        //DEBUG
        BoxMesh bm = (BoxMesh)hitboxInstance.GetNode<MeshInstance3D>("MeshInstance3D").Mesh;
        bm.Size = characterHitbox.Size;
        

        hitboxInstance.GlobalPosition = new Vector3(User.c.GlobalPosition.X, User.c.GlobalPosition.Y + (characterHitbox.Size.Y/2), User.c.GlobalPosition.Z);
        hitboxInstance.GlobalPosition -= User.c.GlobalBasis.Z * (characterHitbox.Size.Z);
        hitboxInstance.BodyEntered += Collision;

        SetProcess(true);
    }

    public override void Interrupt(AttackInfo attackInfo)
    {
        User.c.target = User.c.GlobalPosition;
        User.c.Velocity = Vector3.Zero;
        base.Interrupt(attackInfo);
    }

    public override void End()
    {
        
        base.End();
        User.c.Velocity = Vector3.Zero;
        User.c.target = User.c.GlobalPosition;
        hitboxInstance.QueueFree();
        GD.Print(hitboxInstance);
    }

    public override void _Ready()
    {
        SetProcess(false);
    }
    public override void _Process(double delta)
    {
       
        if (User.c.GlobalPosition.DistanceTo(direction) < User.c.minDistanceFromTarget)
        {
            User.c.Velocity = Vector3.Zero;
            End();
            return;
        }
        User.c.Move((float)delta);
        User.c.Animate();
    }

    

    public void Collision(Node3D body)
    {
        if (body == (Node3D)User.c)
        {
            return;
        }

        End();
    }
}
