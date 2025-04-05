using Godot;

public partial class LevelEnededScreen : Control
{
	[Export] private Button _autoSelectButton;
	private PackedScene _gameScene { get; set; }
	private PackedScene _menuScene { get; set; }
	[Export] private Label bestTimeLabel { get; set; }

    public override void _Ready()
    {
		_autoSelectButton.GrabFocus();

		_gameScene = ResourceLoader.Load<PackedScene>("Scenes/Levels/Main.tscn");
		_menuScene = ResourceLoader.Load<PackedScene>("Scenes/Menus/MainMenu.tscn");
    }


	public void Init()
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

		string bestTimeText =  "Best time: " + (int)(bestTime / 60) + ":" + (int)(bestTime % 60);

		bestTimeLabel.Text = bestTimeText;
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
