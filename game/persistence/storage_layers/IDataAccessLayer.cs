namespace Bombino.game.persistence.storage_layers;

internal interface IDataAccessLayer
{
    void SaveData();

    bool LoadData();
}