using Godot;
using System;

public partial class Goal : Area3D
{
	[Export] private GameManager _gameManager { get; set; }

	public void OnPlayerEnter(Node3D node)
	{
		_gameManager.OnPlayerEnterGoal();
	}
}
