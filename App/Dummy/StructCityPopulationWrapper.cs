using System;
using System.Collections.Generic;
using App.Data;
using FireMapper.Wrapper;

namespace App.Dummy
{
    public class StructCityPopulationWrapper : IPropertyWrapper
    {
        public string Name()
        {
            return "Population";
        }

        public bool IsKey()
        {
            return false;
        }
        
        public object Value(object obj)
        {
            StructCity city = (StructCity) obj;
            return city.Population;
        }

        public object GetValue(Dictionary<string, object> dictionary)
        {
            if(!dictionary.ContainsKey("Population")) return null;
            return Convert.ChangeType(dictionary["Population"], typeof(int));
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