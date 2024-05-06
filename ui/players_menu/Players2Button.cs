using Bombino.game;

namespace Bombino.ui.play_menu;

/// <summary>
/// Represents a button that allows the user to select the number of players as 2.
/// </summary>
internal partial class Players2Button : PlayerButton
{
    #region MethodsForSignals

    /// <summary>
    /// Event handler for the button press event.
    /// </summary>
    public void OnPressed()
    {
        GameManager.NumberOfPlayers = 2;

        _OnPressed();
    }

    #endregion
}