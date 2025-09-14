namespace Game.Core.Save
{
    public interface ISaveService
    {
        void SaveString(string key, string json);
        string LoadString(string key, string defaultValue = "");
        void DeleteKey(string key);
    }
}