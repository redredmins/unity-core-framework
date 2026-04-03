namespace RedMinS
{
    public interface IDataStore
    {
        void Save<T>(string key, T data) where T : class;
        T Load<T>(string key) where T : class;
        bool HasKey(string key);
        void Delete(string key);
        void DeleteAll();
    }
}
