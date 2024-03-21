using Godot;
using System;
using System.Text.RegularExpressions;

public partial class CameraControls : Node3D
{
	Game game;
	Camera3D cam;
    PartyManager pm;
	CharacterBody3D center;
	private bool centered = false;
	[Export] private Vector2 MoveCornersSize;
	[Export] private float camSpeed;
     
	private bool EdgePush = true;


    public override void _Notification(int what)
    {
        switch ((long)what)
		{
			case NotificationWMMouseEnter:
			EdgePush = true;
			break;

			case NotificationWMMouseExit:
			EdgePush = false;
			break;
		}
		   
    }

    public override void _Ready()
	{
		game = GetNode<Game>("/root/Game");
		cam = GetNode<Camera3D>("Camera3D");
		pm = GetNode<PartyManager>("%PartyManager");
	}

	DateTime lastUpdated = DateTime.Now;
    double unscaledDelta;

    public override void _Process(double delta)
    {
       var now = DateTime.Now;
       unscaledDelta = (now - lastUpdated).TotalSeconds;
       lastUpdated = now;
    }

    public override void _PhysicsProcess(double delta)
    {
        Move((float)unscaledDelta);
    }

    void Move(float delta)
	{
		if (Input.IsActionJustPressed("MiddleClick") && pm.leader != -1)
		{
			centered = true;
			center = pm.selectedCharacters[pm.leader];
		}

		if (centered)
		{   
			bool teleportToTarget;
            if (Position.DistanceTo(center.Position) < 2) teleportToTarget = true;
            else teleportToTarget = false;

            if (teleportToTarget) Position = center.Position;
            else Position = Position.MoveToward(center.Position, camSpeed * delta);
		}
        
        if (!EdgePush) return;
        
		Vector2 mousePos = GetViewport().GetMousePosition();
		Vector2 screenSize = (GetViewport().GetVisibleRect().Size);
		Vector2 normalizedPos = new Vector2((mousePos.X/screenSize.X), (mousePos.Y/screenSize.Y));
		
		Vector3 dir = new Vector3(normalizedPos.X - 0.5f, normalizedPos.Y - 0.5f, 0f);
		
		if (Mathf.Abs(dir.X) > (0.5f - MoveCornersSize.X) || Mathf.Abs(dir.Y) > (0.5f - MoveCornersSize.Y))
		{
			centered = false;
			Position += -dir.Normalized() * camSpeed * delta;
		}

	}
	

	public Godot.Collections.Dictionary ShootRay(uint collisionMask)
	{
        if (game.hoveringButton == true) return null; 

		Vector2 mousePos = GetViewport().GetMousePosition();
        var rayLen = 1000f;

		var from = cam.ProjectRayOrigin(mousePos);
		var to = from + (cam.ProjectRayNormal(mousePos) * rayLen);

		var space = GetWorld3D().DirectSpaceState;
		var query = PhysicsRayQueryParameters3D.Create(from, to, collisionMask);
		var result = space.IntersectRay(query);

		if (result.Count == 0) return null;
		return result;
	}
}  
