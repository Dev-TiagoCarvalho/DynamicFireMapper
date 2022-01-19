using System;
using System.Collections.Generic;
using System.Reflection;

namespace FireMapper.Wrapper
{
    public class PropertyComplex : IPropertyWrapper
    {
        private readonly IDataMapper _mapper;
        private readonly PropertyInfo _info;
        private readonly PropertyInfo _nextProperty;
        private readonly bool _isKey;

        public PropertyComplex(PropertyInfo info, String key, IDataMapper mapper)
        {
            this._info = info;
            if(key is not null && key.Equals(info.Name)) _isKey = true;
            else _isKey = false;
            _mapper = mapper;
            _nextProperty = _info.PropertyType.GetProperty(mapper.Key);
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
            return _mapper.GetById(dictionary[_info.Name]);
        }

        public object AddValue(object obj)
        {
            object value = _info.GetValue(obj);
            if(value is null) return null;
            _mapper.Add(value);
            return value.GetType().GetProperty(_mapper.Key)?.GetValue(value);
        }

        public object UpdateValue(object obj)
        {
            object value = _info.GetValue(obj);
            if(value is null) return null;
            _mapper.Update(value);
            return value.GetType().GetProperty(_mapper.Key)?.GetValue(value);
        }

        public void DeleteValue(object keyValue, string collection)
        {
            if (keyValue is null) return;
            if(collection.Equals(_mapper.Collection))
            {
                object aux = _nextProperty.GetValue(keyValue);
                if(aux is null) return;
                _mapper.Delete(aux);
            }
        }
    }
}