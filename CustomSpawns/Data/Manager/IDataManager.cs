namespace Data.Manager
{
    public interface IDataManager<T>
    {
        T Data
        {
            get;
        }
    }
}