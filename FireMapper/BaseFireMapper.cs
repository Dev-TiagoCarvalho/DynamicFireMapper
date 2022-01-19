using System;
using System.Collections.Generic;
using System.Reflection;
using FireMapper.Wrapper;
using FireSource;

namespace FireMapper
{
    public class BaseFireMapper : IDataMapper
    {
        public string Collection { get; protected init; }
        public string Key { get; protected init; }
        
        protected Type Domain;
        protected IDataSource Source;

        protected bool IsValueType;
        protected PropertyInfo KeyProperty;
        protected IPropertyWrapper[] DomainProperties;
        protected ConstructorInfo DomainConstructor;
        
        private readonly List<object> _keyCache = new List<object>();
        
        private object Get(Dictionary<string, object> dictionary)
        {
            if (dictionary is null) return null;
            object[] ctorArgs = new object[DomainProperties.Length];
            for(int i = 0; i < DomainProperties.Length; ++i)
            {
                if(DomainProperties[i] is null) continue;
                ctorArgs[i] = DomainProperties[i].GetValue(dictionary);
                if(DomainProperties[i].IsKey() && ctorArgs[i] is null) return null;
                if(DomainProperties[i].IsKey() && !_keyCache.Contains(ctorArgs[i])) _keyCache.Add(ctorArgs[i]);
            }
            return DomainConstructor.Invoke(ctorArgs);
        }

        private object GetInstanceOrValue(object obj)
        {
            if (IsValueType && obj is null) return Activator.CreateInstance(Domain);
            return obj;
        }
        
        public IEnumerable<object> GetAll()
        {
            IEnumerable<Dictionary<string, object>> list = Source.GetAll();
            if(list is null) return new object[0];
            List<object> toReturn = new List<object>();
            foreach(var dict in list)
            {
                object obj = Get(dict);
                if(obj is not null) toReturn.Add(GetInstanceOrValue(obj));
            }
            return toReturn;
        }

        public object GetById(object keyValue)
        {
            Dictionary<string, object> dictionary = Source.GetById(keyValue);
            return GetInstanceOrValue(Get(dictionary));
        }

        public void Add(object obj)
        {
            if(obj is null) return;
            object keyValue = KeyProperty.GetValue(obj);
            if(_keyCache.Contains(keyValue)) throw new Exception($"An object with key value: {keyValue}, already exists in DataBase");
            object aux = GetById(keyValue);
            if(aux is not null && KeyProperty.GetValue(aux) is not null) throw new Exception($"An object with key value: {keyValue}, already exists in DataBase");
            
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach(IPropertyWrapper property in DomainProperties)
            {
                if(property is null) continue;
                dictionary[property.Name()] = property.AddValue(obj);
            }
            _keyCache.Add(keyValue);
            Source.Add(dictionary);
        }

        public void Update(object obj)
        {
            if(obj is null) return;
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach(IPropertyWrapper property in DomainProperties)
            {
                if(property is null) continue;
                dictionary[property.Name()] = property.UpdateValue(obj);
            }
            Source.Update(dictionary);
        }

        public void Delete(object keyValue)
        {
            if(keyValue is null) return;
            object value = GetById(keyValue);
            foreach(IPropertyWrapper property in DomainProperties)
            {
                if(property is null) continue;
                property.DeleteValue(property.Value(value), Collection);
            }

            if(_keyCache.Contains(keyValue)) _keyCache.Remove(keyValue);
            Source.Delete(keyValue);
        }
    }
}