namespace Bombino.game.persistence.storage_layers.key_binds;

/// <summary>
/// Represents a key binds settings data access layer.
/// </summary>
internal interface ISettingsKeyBinds
{
    /// <summary>
    /// Saves the key binds data.
    /// </summary>
    /// <returns>True if the data was successfully saved; otherwise, false.</returns>
    bool SaveKeyBinds();

    /// <summary>
    /// Loads the key binds data.
    /// </summary>
    /// <returns>True if the data was successfully loaded; otherwise, false.</returns>
    bool LoadKeyBinds();

    /// <summary>
    /// Checks if the key binds data is loaded.
    /// </summary>
    /// <returns>
    /// True if the key binds data is loaded; otherwise, false.
    /// </returns>
    bool IsLoaded();
}
