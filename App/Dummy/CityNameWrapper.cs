using System.Collections.Generic;
using App.Data;
using FireMapper.Wrapper;

namespace App.Dummy
{
    public class CityNameWrapper : IPropertyWrapper
    {
        public string Name()
        {
            return "Name";
        }

        public bool IsKey()
        {
            return true;
        }

        public object Value(object obj)
        {
            City city = (City) obj;
            return city.Name;
        }

        public object GetValue(Dictionary<string, object> dictionary)
        {
            if(!dictionary.ContainsKey("Name")) return null;
            return dictionary["Name"];
        }

        public object AddValue(object obj)
        {
            return Value(obj);
        }

        public object UpdateValue(object obj)
        {
            return Value(obj);
        }

        public void DeleteValue(object keyValue, string collection)
        {
            
        }
    }
}