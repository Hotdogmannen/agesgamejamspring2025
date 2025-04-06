using System;
using Godot;

public partial class GameManager : Node3D
{	
	[Signal] public delegate void OnCountDownFinishedEventHandler();
	/// <summary>
	/// type == 0: Won,
	/// type == 1: Lose,
	/// type == 2: TimesUp
	/// </summary>
	[Signal] public delegate void OnLevelEndedEventHandler(int type, float time);

	private PackedScene _winScreen { get; set; }
	private PackedScene _timesUpScreen { get; set; }
	private PackedScene _loseScreen { get; set; }

	[ExportCategory("Countdown")]
	[Export] private float _countDownTime = 5.0f;
	[Export] private Label _countDownLabel;
	[Export] private TextureRect _countDownImage;
	[Export] private float countDownMinScale;
	[Export] private float countDownMaxScale;
	[Export] private float countDownGoScale;
	[Export] private Texture2D[] countdownImages;

	[ExportCategory("Level timer")]
	[Export] private Timer _levelTimer;
	[Export] private Label _levelTimerLabel;
	[Export] private float _levelTime = 300;

	private float _maxTime;
	private bool _isCountdown = false;
	private bool _hasGhostShipFinished = false;
	private bool _panic = false;
	private bool _levelEnded = false;

    public override void _Ready()
    {
		_winScreen = ResourceLoader.Load<PackedScene>("Scenes/Menus/WinScreen.tscn");
		_timesUpScreen = ResourceLoader.Load<PackedScene>("Scenes/Menus/TimesUpScreen.tscn");
		_loseScreen = ResourceLoader.Load<PackedScene>("Scenes/Menus/LoseScreen.tscn");

		_levelTimer.WaitTime = _levelTime;
		_isCountdown = true;
		_maxTime = _countDownTime;
		_countDownImage.Scale = Vector2.One * countDownMinScale;
    }

    public override void _Process(double delta)
    {
		if(Input.IsKeyPressed(Key.P) && !_panic)
		{
			_panic = true;
			EmitSignal(SignalName.OnLevelEnded, 2, (float)_levelTimer.TimeLeft);
			OnTimerFinished();
		}

		CountDown((float)delta);

		UpdateLevelTimer();
    }

	private void UpdateLevelTimer()
	{
		if(_levelTimer.TimeLeft > 0)
		{
			string timeText = "";
			timeText += (int)_levelTimer.TimeLeft / 60 + ":";
			
			int sec = (int)_levelTimer.TimeLeft % 60;
			if(sec < 10)
				timeText += "0";
			timeText += sec;

			_levelTimerLabel.Text = timeText;
		}
	}

	private void CountDown(float delta)
	{
		if(!_isCountdown)
			return;

		_countDownTime -= delta;

		if(_countDownTime >= 0) {
			_countDownImage.Texture = countdownImages[Mathf.FloorToInt(_countDownTime)];
			_countDownImage.Scale = (Vector2.One * countDownMaxScale).Lerp(Vector2.One * countDownMinScale,(_countDownTime/_maxTime));
		}

		if(_countDownTime <= 1)
		{
			_countDownImage.Scale = Vector2.One * countDownGoScale;
			_countDownLabel.Text = "GO!!";
			EmitSignal(SignalName.OnCountDownFinished);
			_levelTimer.Start();

			if(_countDownTime <= 0)
			{
				_countDownImage.Visible = false;
				OnCountDownFinishedFunc();
			}
		}
		
	}

	public void OnShipReachedEnd()
	{
		_hasGhostShipFinished = true;
	}

	private void OnCountDownFinishedFunc()
	{
		_countDownLabel.Text = "";
		_isCountdown = false;
	}

	public void OnPlayerEnterGoal()
	{
		if(_levelEnded)
			return;

		if(_hasGhostShipFinished)
		{
			EmitSignal(SignalName.OnLevelEnded, 1, (float)(_levelTimer.WaitTime - _levelTimer.TimeLeft));
			LevelEnededScreen screen =_loseScreen.Instantiate<LevelEnededScreen>();
			screen.Init((float)(_levelTimer.WaitTime - _levelTimer.TimeLeft));
			AddChild(screen);
		}
		else
		{
			EmitSignal(SignalName.OnLevelEnded, 0, (float)(_levelTimer.WaitTime - _levelTimer.TimeLeft));
			LevelEnededScreen screen =_winScreen.Instantiate<LevelEnededScreen>();
			screen.Init((float)(_levelTimer.WaitTime - _levelTimer.TimeLeft));
			AddChild(screen);
		}

		_levelTimer.Stop();
		_levelEnded = true;
	}

	public void OnTimerFinished()
	{
		if(_levelEnded)
			return;
		_levelEnded = true;

		EmitSignal(SignalName.OnLevelEnded, 2, (float)(_levelTimer.WaitTime - _levelTimer.TimeLeft));
		LevelEnededScreen screen =_timesUpScreen.Instantiate<LevelEnededScreen>();
		screen.Init((float)(_levelTimer.WaitTime - _levelTimer.TimeLeft));
		AddChild(screen);
		_levelTimer.Stop();
	}
}
