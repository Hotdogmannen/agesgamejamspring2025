using System.Collections.Generic;
using Godot;

public partial class GhostShip : Node3D
{
	[Signal] public delegate void OnShipReachedEndEventHandler();

	[Export] private float _positionDifference = 0.1f;
	[Export] public float oarSpeed {get; set;}

	[ExportCategory("Animation")]
	[Export] public float BobbingSpeed {get; set;}
    [Export] public float BobbingVelocitySpeedupPercentage {get; set;}
    [Export] public float BobbingMagnitude {get; set;}
	[Export] public float OarAnimationHeight {get; set;}
    [Export] public float OarAnimationWidth {get; set;}
    [Export] public float OarAnimationSharpness {get; set;}

	[ExportCategory("Oars")]
	[Export] Node3D oarLeft { get; set; }
    [Export] Node3D oarRight { get; set; }

	private List<Vector3> _playerPositions;
	
	private bool _canMove = false;
	private float _currentTime = 0.0f;
	private float _timeBetweenSaves = 0.0f;
	private double animationTime;

    public void Init(List<Vector3> positions, float timeBetweenSaves)
	{
		if(positions == null)
			return;

		_playerPositions = positions;
		_timeBetweenSaves = timeBetweenSaves;

		Position = _playerPositions[0];
	}

	public void StartGhost()
	{
		if(_playerPositions == null)
			return;
		
		_canMove = true;
	}

    public override void _Process(double delta)
    {
		ApplyBobbingAnimation(delta);
		UpdateOarAnimation(delta);

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
			EmitSignal(SignalName.OnShipReachedEnd);
			return;
		}

		float lerpTime = (_currentTime - startIndex*_timeBetweenSaves)/_timeBetweenSaves;
		Vector3 newPos = _playerPositions[startIndex].Lerp(_playerPositions[endIndex], lerpTime);

		//RotationDegrees = RotationDegrees.Lerp(RotationDegrees.Rotated(Vector3.Up, Position.AngleTo(newPos)), _currentTime);

		//RotationDegrees.Rotated(Vector3.Up, Position.AngleTo(newPos));

		//LookAt(Position.Lerp(newPos, _currentTime));

		//Rotate(Vector3.Up, Position.SignedAngleTo(newPos, Vector3.Up));

		Position = newPos;
	}

	private void UpdateOarAnimation(double delta)
    {
		SetOarRotation(delta, oarLeft, _currentTime * oarSpeed);
		SetOarRotation(delta, oarRight, _currentTime * oarSpeed, true);
    }

	private void SetOarRotation(double delta,Node3D oar, float power,bool flipped = false){
        var target = new Vector3(
            0.1f,
            (float)((flipped? -1 : 1)*Mathf.Cos(power*2*Mathf.Pi) * OarAnimationWidth),
            (float)((flipped? 1 : -1)*Mathf.Sin(power*2*Mathf.Pi) * OarAnimationHeight));

        float weight = 1f - Mathf.Exp(-OarAnimationSharpness * (float)delta);
        oar.RotationDegrees = oar.RotationDegrees.Lerp(target,weight);

    }

	private void ApplyBobbingAnimation(double delta)
	{    
        animationTime += delta;

        var target = new Vector3(
            (float)Mathf.Cos(animationTime*BobbingSpeed) * BobbingMagnitude,
            0f,
            (float)Mathf.Sin(animationTime*BobbingSpeed+0.2f) * BobbingMagnitude);
        RotationDegrees = target;
    }

	public static float Fade(float t) => t * t * t * (t * (t * 6 - 15) + 10);   
}