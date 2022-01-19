

using System.Collections.Generic;

namespace FireMapper
{
    public interface IDataMapper
    {
        string Collection { get;  }
        string Key { get;  }
        public IEnumerable<object> GetAll();
        public object GetById(object keyValue);
        public void Add(object obj);
        public void Update(object obj);
        public void Delete(object keyValue);
    }
}