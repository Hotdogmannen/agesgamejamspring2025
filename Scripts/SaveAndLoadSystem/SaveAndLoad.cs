using Godot;
using System.Collections.Generic;

public partial class SaveAndLoad : Node
{
	public const string SAVE_FILE_PATH = "user://savegame.save";

	[Export] private Node3D _target { get; set; }
	[Export] private float _timeBetweenSaves { get; set; } = 1.0f;
	[Export] private GhostShip _ghostShip { get; set; }
	[Export] private bool _ShouldSave = true;
	private List<Vector3> _playerPositions = new List<Vector3>();

	private bool _isSaving = false;

	private float _timer = 0.0f;

    public override void _Ready()
    {
		_ghostShip.Init(LoadData(), _timeBetweenSaves);
    }

	public void StartSaving()
	{
		_isSaving = true;
	}

	public void StopSaving()
	{
		_isSaving = false;

		SaveData();

		ClearData();
	}

	public override void _Process(double delta)
	{
		if(!_isSaving)
			return;

		if(_timer >= _timeBetweenSaves)
		{
			_playerPositions.Add(_target.Position);
			_timer = 0.0f;
		}
		else
			_timer += (float)delta;
	}

	public void SaveData()
	{
		if(!_ShouldSave)
			return;
			
		if(_target == null)
		{
			GD.PrintErr("No target assigned!");
			return;
		}

		using FileAccess file = FileAccess.Open(SAVE_FILE_PATH, FileAccess.ModeFlags.Write);
		
		foreach (Vector3 pos in _playerPositions)
		{
			string savedPos = Json.Stringify(pos);
			file.StoreLine(savedPos);
		}

		file.Close();
	}
	
	public List<Vector3> LoadData()
	{
		if (!FileAccess.FileExists(SAVE_FILE_PATH))
    	{
			GD.PrintErr("Save file not found at: " + SAVE_FILE_PATH);
        	return null;
    	}
		
		using FileAccess file = FileAccess.Open(SAVE_FILE_PATH, FileAccess.ModeFlags.Read);

		List<Vector3> positions = new();

		while (file.GetPosition() < file.GetLength())
		{
			string jsonString = file.GetLine();

        	Json json = new Json();
        	Error parseResult = json.Parse(jsonString);
        	if (parseResult != Error.Ok)
        	{
        	    GD.PrintErr($"JSON Parse Error: {json.GetErrorMessage()} in {jsonString} at line {json.GetErrorLine()}");
        	    continue;
        	}
			
			positions.Add((Vector3)GD.StrToVar("Vector3" + json.Data.AsString()));
		}

		return positions;
	}

	public void ClearData()
	{
		_playerPositions.Clear();
		_timer = 0.0f;
	}
}
