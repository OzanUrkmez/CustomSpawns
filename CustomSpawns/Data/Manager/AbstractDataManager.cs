using System;

namespace Data.Manager
{
    public abstract class AbstractDataManager<T, D> : IDataManager<D> where T : class
    {

        private static T _instance = null!;

        public static T Instance { get => _instance; }

        public static void Init()
        {
            if (_instance == null)
            {
                _instance = (new Lazy<T>(() => CreateInstanceOfT())).Value;
            }

            int d;
        }
        
        private static T CreateInstanceOfT()
        {
            return Activator.CreateInstance(typeof(T), true) as T;
        }
        
        public D Data { get; protected set; }
    }
}