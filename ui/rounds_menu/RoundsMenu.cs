using Bombino.game;
using Godot;

namespace Bombino.ui.rounds_menu;

/// <summary>
/// Represents a user interface component for selecting the number of rounds in a game.
/// </summary>
internal partial class RoundsMenu : CanvasLayer
{
    #region Exports

    [Export(PropertyHint.File, "*.tscn")]
    private string LoadingScenePath { get; set; }

    #endregion

    #region Fields

    private LineEdit _numberInput;
    private PanelContainer _errorContainer;
    private Label _errorLabel;

    #endregion

    #region Overrides

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        _numberInput = GetNode<LineEdit>(
            "RoundsContainer/MarginContainer/GridContainer/NumberInput"
        );

        _errorContainer = GetNode<PanelContainer>("ErrorContainer");
        _errorLabel = _errorContainer.GetNode<Label>("ErrorLabel");
    }

    #endregion

    #region MethodsForSignals

    /// <summary>
    /// Event handler for when the number input is submitted.
    /// </summary>
    /// <param name="newText">The new text typed in the number input.</param>
    private void OnNumberInputSubmitted(string newText)
    {
        if (!IsTextValidInteger(newText, out var number))
        {
            EmptyTextAndShowError("Please enter a valid number");
            return;
        }

        if (!IsNumberInRange(number, 1, 10))
        {
            EmptyTextAndShowError("Please enter a number between 1 and 10");
            return;
        }

        HideError();

        GameManager.NumberOfRounds = number;
        GetTree().ChangeSceneToFile(LoadingScenePath);
    }

    #endregion

    /// <summary>
    /// Checks if the given text is a valid integer.
    /// </summary>
    /// <param name="text">The text to check.</param>
    /// <param name="number">The parsed integer if the text is valid.</param>
    /// <returns>True if the text is a valid integer, false otherwise.</returns>
    private static bool IsTextValidInteger(string text, out ushort number)
    {
        return ushort.TryParse(text, out number);
    }

    /// <summary>
    /// Clears the text input and shows an error message.
    /// </summary>
    /// <param name="errorMessage">The error message to display.</param>
    private void EmptyTextAndShowError(string errorMessage)
    {
        _numberInput.Text = string.Empty;

        if (!_errorContainer.Visible)
        {
            SetAndShowErrorMessage(errorMessage);
            return;
        }

        SetErrorMessage(errorMessage);
    }

    /// <summary>
    /// Sets the error message and shows the error container.
    /// </summary>
    /// <param name="errorMessage">The error message to display.</param>
    private void SetAndShowErrorMessage(string errorMessage)
    {
        SetErrorMessage(errorMessage);
        _errorContainer.Show();
    }

    /// <summary>
    /// Sets the error message.
    /// </summary>
    /// <param name="errorMessage">The error message to display.</param>
    private void SetErrorMessage(string errorMessage)
    {
        _errorLabel.Text = errorMessage;
    }

    /// <summary>
    /// Hides the error container.
    /// </summary>
    private void HideError()
    {
        if (_errorContainer.Visible)
            _errorContainer.Hide();
    }

    /// <summary>
    /// Checks if a number is within the specified range.
    /// </summary>
    /// <param name="number">The number to check.</param>
    /// <param name="min">The minimum value of the range.</param>
    /// <param name="max">The maximum value of the range.</param>
    /// <returns>True if the number is within the range, false otherwise.</returns>
    private static bool IsNumberInRange(int number, int min, int max)
    {
        return number >= min && number <= max;
    }
}
