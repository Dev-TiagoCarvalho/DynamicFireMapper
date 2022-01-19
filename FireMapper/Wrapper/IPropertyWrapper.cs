using System;
using System.Collections.Generic;
using System.Reflection;

namespace FireMapper.Wrapper
{
    public interface IPropertyWrapper
    {
        public string Name();
        public bool IsKey();
        public object Value(object obj);
        public object GetValue(Dictionary<string, object> dictionary);
        public object AddValue(object obj);
        public object UpdateValue(object obj);
        public void DeleteValue(object keyValue, string collection);
    }
}