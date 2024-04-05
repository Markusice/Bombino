namespace Bombino.scripts.persistence;

internal interface IDataAccessLayer
{
    void SaveData();
    
    bool LoadData();
}
