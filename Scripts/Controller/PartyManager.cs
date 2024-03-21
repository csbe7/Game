using Godot;
using System;

public partial class PartyManager : Node
{
	private CameraControls cam;
	public Godot.Collections.Array<Controller> party = new Godot.Collections.Array<Controller>{};
    public Godot.Collections.Array<Controller> selectedCharacters = new Godot.Collections.Array<Controller>{};
	public int leader;
    [Export] private uint collisionMask;
	bool LeftClickReady;

	
	public override void _Ready()
	{
		cam = GetNode<CameraControls>("%CameraPivot");
		GetParty();
		leader = -1;
	}

	void GetParty()
	{
		var nodes = GetTree().GetNodesInGroup("Player Character");
        foreach(Node n in nodes)
		{
			if (n is Controller) party.Add(GetNode<Controller>(n.GetPath())); 
		}

	}

	
	public override void _Process(double delta)
	{
        GetInput();
	}

	public override void _PhysicsProcess(double delta)
	{
        DoOutput();
	}

	private void GetInput()
	{
        if (Input.IsActionJustPressed("LeftClick"))
		{
			LeftClickReady = true;
		}
	}

	private void DoOutput()
	{
        if (LeftClickReady)
		{
			LeftClick();
		}
	}



	private void LeftClick()
	{
		var result = cam.ShootRay(1);
			if (result != null)
			{
				var body = (PhysicsBody3D)result["collider"];
				if (body != null && body.IsInGroup("Player Character"))
				{
					if (body is Controller)
					{
						Selector((Controller)body);
						//selectedCharacters.Add((Character)body);
						//GD.Print((selectedCharacters[0]));
					}
				}
				else if (leader != -1)
				{
                    selectedCharacters[leader].target = (Vector3)result["position"];
				}
			}
			LeftClickReady = false;
	}

	void Selector(Controller c)
	{
		int mode;
        if (selectedCharacters.Contains(c)) mode = 1;
		else mode = 0;

		switch (mode)
		{
			case 0:  //select
			if (selectedCharacters.Count == 0)
			{
				selectedCharacters.Add(c);
				leader = 0;
				c.leader = null;
				c.target = c.Position;
			}
			else 
			{
                selectedCharacters.Add(c);
				c.leader = selectedCharacters[leader];
			}
			break;

			case 1: //deselect
			int index = selectedCharacters.IndexOf(c);
            selectedCharacters.RemoveAt(index);
			if (selectedCharacters.Count == 0) leader = -1;
			else
			{
				leader = 0;
                selectedCharacters[leader].leader = null;
				selectedCharacters[leader].target = selectedCharacters[leader].Position;
			} 
			break;
		}
	}
}
