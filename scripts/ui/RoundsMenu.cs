namespace Bombino.scripts.ui;

using Godot;

internal partial class RoundsMenu : CanvasLayer
{
    #region Exports

    [Export]
    private PackedScene _mainScene;

    #endregion

    private LineEdit _numberInput;
    private PanelContainer _errorContainer;
    private Label _errorLabel;

    public override void _Ready()
    {
        _numberInput = GetNode<LineEdit>(
            "RoundsContainer/MarginContainer/GridContainer/NumberInput"
        );

        _errorContainer = GetNode<PanelContainer>("ErrorContainer");
        _errorLabel = _errorContainer.GetNode<Label>("ErrorLabel");
    }

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
        GetTree().ChangeSceneToPacked(_mainScene);
    }

    private bool IsTextValidInteger(string text, out int number)
    {
        return int.TryParse(text, out number);
    }

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

    private void SetAndShowErrorMessage(string errorMessage)
    {
        SetErrorMessage(errorMessage);
        _errorContainer.Show();
    }

    private void SetErrorMessage(string errorMessage)
    {
        _errorLabel.Text = errorMessage;
    }

    private void HideError()
    {
        if (_errorContainer.Visible)
            _errorContainer.Hide();
    }

    private static bool IsNumberInRange(int number, int min, int max)
    {
        return number >= min && number <= max;
    }
}
