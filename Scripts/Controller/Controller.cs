using Godot;
using System;

public partial class Controller : CharacterBody3D
{
    public Game game;

    public enum State{
		free,
		busy,
		hitstun,
	}
	public State state = State.free;

	public AnimationNodeStateMachine animationStateMachine;
	public AnimationNodeStateMachinePlayback stateMachine;// = (AnimationNodeStateMachinePlayback)(cs.at.Get("parameters/playback"));

    public CharacterSheet cs;
    public NavigationAgent3D nav;
	

    [ExportCategory("Pathfinfing")]
	public CharacterBody3D leader;
    public Vector3 target = new Vector3();
	
	private float minDistance = 1;
	[Export] public float minDistanceFromTarget = 1.1f;
	[Export] public float minDistanceFromLeader = 5f;

	[ExportCategory("Movement")]
	[Export] public float Speed = 5;
	[Export] public float Acceleration = 10;
	[Export] private float JumpVelocity = 5;
	public bool onFloor;
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle() * 6;
     
    public override void _Ready()
    {
        game = GetNode<Game>("/root/Game");

        nav = GetNode<NavigationAgent3D>("NavigationAgent3D");
        nav.MaxSpeed = Speed;
		nav.TargetDesiredDistance = minDistance;
        
		cs = GetNode<CharacterSheet>("CharacterSheet");

        animationStateMachine = (AnimationNodeStateMachine)cs.at.TreeRoot;
		stateMachine = (AnimationNodeStateMachinePlayback)cs.at.Get("parameters/playback");

		target = Position;		
    }





    public override void _PhysicsProcess(double delta)
	{
		//cs.at. = game.scaledTime;
        

        onFloor = IsOnFloor();
		
		if (state == State.free)
		{
            if (leader != null)
		    {
			   nav.TargetPosition = leader.Position;
               minDistance = minDistanceFromLeader;
		    } 
		    else
		    {
			   nav.TargetPosition = target;
               minDistance = minDistanceFromTarget;
		    }

			Move((float)delta);
            Animate();
		}

        if (state == State.hitstun)
		{
			Friction((float)delta);
		}
        
		Gravity((float)delta);
		MoveAndSlide();
	}

	void Gravity(float delta)
	{
		Vector3 velocity = Velocity;
		if (!onFloor)
		{
			velocity.Y -= gravity * delta;
		}
		
		Velocity = velocity;
	} 
    
	void Friction(float delta)
	{
		if (onFloor)
		{
            float speed = Velocity.Length();
			speed -= delta * 10;
			speed = Mathf.Max(speed, 0);
			Velocity = Velocity.Normalized() * speed;
		}
	}

	public void Move(float delta)
	{
		Vector3 velocity = Velocity;
        if (GlobalPosition.DistanceTo(nav.TargetPosition) < minDistance)
		{
			if (onFloor) Velocity = new Vector3(0f, 0f, 0f);
			else Velocity = new Vector3(0f, Velocity.Y, 0f);
			return;
		}
		
        Vector3 dir;
        
		//point to target
		dir = nav.GetNextPathPosition() - GlobalPosition;
		velocity += dir.Normalized() * Acceleration * 10 * delta;
		if (velocity.Length() > Speed)
		{
			velocity = velocity.Normalized() * Speed;
		} 

		Velocity = velocity;
		
	}

	public void Animate()
	{
		if (onFloor)
		{
			if (Velocity.Length() < 0.1)
		    {
				stateMachine.Travel("Idle");
		    }
		    else
			{
				stateMachine.Travel("Run");
				Vector3 direction = new Vector3(Velocity.X, 0f, Velocity.Z);
                LookAt(direction.Normalized() * 100);
			} 

		}
	}

	public AnimationNodeStateMachineTransition TransitionTo(string targetAnim, float XFade, bool doTransition)
	{
        string currAnim = stateMachine.GetCurrentNode();
		if (animationStateMachine.HasTransition(currAnim, targetAnim))
		{
			if (doTransition) stateMachine.Travel(targetAnim);
			//GD.Print("Got transition");
            return null;
		}
		else 
		{
           AnimationNodeStateMachineTransition transition = new AnimationNodeStateMachineTransition();
		   transition.XfadeTime = XFade;
		   transition.SwitchMode = AnimationNodeStateMachineTransition.SwitchModeEnum.Sync;

		   if (doTransition) stateMachine.Travel(targetAnim);

		   return transition;
		}
	}

}



