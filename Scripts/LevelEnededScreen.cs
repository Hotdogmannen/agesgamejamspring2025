using Godot;

public partial class LevelEnededScreen : Control
{
	[Export] private Button _autoSelectButton;
	private PackedScene _gameScene { get; set; }
	private PackedScene _menuScene { get; set; }
	[Export] private Label bestTimeLabel { get; set; }
	[Export] private Label _currentTimeLabel { get; set; }

    public override void _Ready()
    {
		_autoSelectButton.GrabFocus();

		_gameScene = ResourceLoader.Load<PackedScene>("Scenes/Levels/Main.tscn");
		_menuScene = ResourceLoader.Load<PackedScene>("Scenes/Menus/MainMenu.tscn");
    }

	public void Init(float currentTime)
	{
		if (!FileAccess.FileExists(SaveAndLoad.TIME_SAVE_FILE_PATH))
    	{
			GD.PrintErr("Save file not found at: " + SaveAndLoad.TIME_SAVE_FILE_PATH);
        	return;
    	}
		
		using FileAccess file = FileAccess.Open(SaveAndLoad.TIME_SAVE_FILE_PATH, FileAccess.ModeFlags.Read);

        Json json = new Json();
        Error parseResult = json.Parse(file.GetLine());
        if (parseResult != Error.Ok)
        {
            GD.PrintErr($"JSON Parse Error: {json.GetErrorMessage()} in {file.GetLine()} at line {json.GetErrorLine()}");
            return;
        }

		float bestTime = (float)json.Data;

		string bestTimeText = "";
		if(((int)(bestTime % 60)) > 9)
			bestTimeText =  "Best Time: " + (int)(bestTime / 60) + ":" + (int)(bestTime % 60);
		else
			bestTimeText =  "Best Time: " + (int)(bestTime / 60) + ":" + "0" + (int)(bestTime % 60);

		bestTimeLabel.Text = bestTimeText;

		string currentTimeText = "";
		if(((int)(currentTime % 60)) > 9)
			currentTimeText =  "Your Time: " + (int)(currentTime / 60) + ":" + (int)(currentTime % 60);
		else
			currentTimeText =  "Your Time: " + (int)(currentTime / 60) + ":" + "0" + (int)(currentTime % 60);

		_currentTimeLabel.Text = currentTimeText;
	}

	public void OnRestart()
	{
		GetTree().ChangeSceneToPacked(_gameScene);
	}

	public void OnMainMenu()
	{
		GetTree().ChangeSceneToPacked(_menuScene);
	}
}
