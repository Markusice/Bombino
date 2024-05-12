namespace Bombino.game.persistence.storage_layers.key_binds;

/// <summary>
/// Represents a key binds settings data access layer.
/// </summary>
internal interface ISettingsKeyBinds
{
    bool SaveKeyBinds();

    bool LoadKeyBinds();
}
