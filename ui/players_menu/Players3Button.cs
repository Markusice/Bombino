using Bombino.game;
using Bombino.ui.scripts;

namespace Bombino.ui.players_menu;

/// <summary>
/// Represents a button for selecting 3 players in the UI.
/// </summary>
internal partial class Players3Button : PlayerButton, IUiButton
{
    #region MethodsForSignals

    /// <summary>
    /// Event handler for the button press event.
    /// </summary>
    public void OnPressed()
    {
        GameManager.NumberOfPlayers = 3;

        _OnPressed();
    }

    #endregion
}
