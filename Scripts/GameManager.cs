using Godot;

public partial class GameManager : Node3D
{	
	[Signal] public delegate void OnCountDownFinishedEventHandler();

	[ExportCategory("Countdown")]
	[Export] private float _countDownTime = 5.0f;
	[Export] private Label _countDownLabel;

	[ExportCategory("Level timer")]
	[Export] private Timer _levelTimer;
	[Export] private Label _levelTimerLabel;

	private bool _isCountdown = false;

    public override void _Ready()
    {
		_isCountdown = true;
    }

    public override void _Process(double delta)
    {
		CountDown((float)delta);
    }

	private void CountDown(float delta)
	{
		if(_levelTimer.TimeLeft >= 0)
		{

		}
		else
		{
			
		}

		if(!_isCountdown)
			return;

		_countDownTime -= delta;

		if(_countDownTime <= 1)
		{
			_countDownLabel.Text = "GO!!";

			if(_countDownTime <= 0)
			{
				OnCountDownFinishedFunc();
			}
		}
		else
		{
        	_countDownLabel.Text = Mathf.FloorToInt(_countDownTime).ToString();
		}
	}

	private void OnCountDownFinishedFunc()
	{
		_countDownLabel.Text = "";
		_isCountdown = false;
		EmitSignal(SignalName.OnCountDownFinished);
		_levelTimer.Start();
	}

	public void OnTimerFinished()
	{

	}
}
