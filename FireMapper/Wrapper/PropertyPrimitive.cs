using System;
using System.Collections.Generic;
using System.Reflection;

namespace FireMapper.Wrapper
{
    public class PropertyPrimitive : IPropertyWrapper
    {
        private readonly PropertyInfo _info;
        private readonly bool _isKey;
        
        public PropertyPrimitive(PropertyInfo info, String key)
        {
            _info = info;
            if(key is not null && key.Equals(info.Name)) _isKey = true;
            else _isKey = false;
        }

        public string Name()
        {
            return _info.Name;
        }

        public bool IsKey()
        {
            return _isKey;
        }

        public object Value(object obj)
        {
            return _info.GetValue(obj);
        }
        
        public object GetValue(Dictionary<string, object> dictionary)
        {
            if(!dictionary.ContainsKey(_info.Name)) return null;
            return Convert.ChangeType(dictionary[_info.Name], _info.PropertyType);
        }

        public object AddValue(object obj)
        {
            return _info.GetValue(obj);
        }

        public object UpdateValue(object obj)
        {
            return _info.GetValue(obj);
        }

        public void DeleteValue(object keyValue, string collection) { }
    }
}