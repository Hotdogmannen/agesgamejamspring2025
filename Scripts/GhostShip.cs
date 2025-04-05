using System.Collections.Generic;
using Godot;

public partial class GhostShip : Node3D
{
	[Export] private float _positionDifference = 0.1f;

	private List<Vector3> _playerPositions;
	private int _pathIndex = 1;
	private bool _canMove = false;
	private float _currentTime = 0.0f;
	private float _timeBetweenSaves = 0.0f;

    public void Init(List<Vector3> positions, float timeBetweenSaves)
	{
		_playerPositions = positions;
		_timeBetweenSaves = timeBetweenSaves;

		Position = _playerPositions[0];
	}

	public void StartGhost()
	{
		_canMove = true;
	}

    public override void _Process(double delta)
    {
		if(!_canMove)
			return;

		_currentTime += (float)delta;

		int startIndex = 0;
		int endIndex = 0;

		float time = _currentTime / _timeBetweenSaves;
		startIndex = Mathf.FloorToInt(time);
		endIndex = Mathf.CeilToInt(time);

		if(endIndex > _playerPositions.Count - 1)
		{
			_canMove = false;
			return;
		}

		float lerpTime = (_currentTime - startIndex*_timeBetweenSaves)/_timeBetweenSaves;
		Vector3 newPos = _playerPositions[startIndex].Lerp(_playerPositions[endIndex], lerpTime);

		Position = newPos;
	}
}