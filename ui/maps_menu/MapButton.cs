using Bombino.game;
using Bombino.map;
using Bombino.ui.scripts;
using Godot;

namespace Bombino.ui.maps_menu;

/// <summary>
/// Represents the map button component.
/// </summary>
internal abstract partial class MapButton : Button, IUiButton
{
    #region Exports

    [Export(PropertyHint.File, "*.tscn")]
    private string _roundsMenuScenePath;

    #endregion

    #region Fields

    protected MapType MapType { get; set; }

    #endregion

    #region Overrides

    public override void _Ready()
    {
        Pressed += OnPressed;

        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
    }

    #endregion

    #region MethodsForSignals

    /// <summary>
    /// Called when the map button is pressed.
    /// Sets the selected map and changes the scene to the rounds menu.
    /// </summary>
    public void OnPressed()
    {
        SetSelectedMapAndChangeScene();

        GetTree().ChangeSceneToFile(_roundsMenuScenePath);
    }

    /// <summary>
    /// Called when the mouse enters the button.
    /// </summary>
    private void OnMouseEntered()
    {
        if (Material is ShaderMaterial shaderMaterial)
        {
            shaderMaterial.SetShaderParameter("is_hovered", true);
        }
    }

    /// <summary>
    /// Called when the mouse exits the button.
    /// </summary>
    private void OnMouseExited()
    {
        if (Material is ShaderMaterial shaderMaterial)
        {
            shaderMaterial.SetShaderParameter("is_hovered", false);
        }
    }

    #endregion

    /// <summary>
    /// Sets the selected map and changes the scene.
    /// </summary>
    private void SetSelectedMapAndChangeScene()
    {
        GameManager.SelectedMap = MapType;
    }
}
