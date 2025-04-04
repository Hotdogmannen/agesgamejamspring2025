using System.Collections.Generic;
using Godot;

public partial class GhostShip : Node3D
{
	[Export] private float _positionDifference = 0.1f;

	private List<Vector3> _playerPositions;
	private int _pathIndex = 1;
	private bool _canMove = false;
	private float _lerpTime = 0.0f;
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

		if(_playerPositions[_pathIndex].DistanceTo(Position) < _positionDifference)
		{
			_pathIndex++;
			_lerpTime = 0.0f;
			if(_playerPositions.Count-1 < _pathIndex)
			{
				_canMove = false;
				return;
			}
		}

		_lerpTime += (float)delta;

		Position = Position.Lerp(_playerPositions[_pathIndex], _lerpTime/_timeBetweenSaves);
    }
}
