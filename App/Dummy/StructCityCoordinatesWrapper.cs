using System.Collections.Generic;
using App.Data;
using FireMapper;
using FireMapper.Wrapper;

namespace App.Dummy
{
    public class StructCityCoordinatesWrapper : IPropertyWrapper
    {
        private readonly IDataMapper _mapper;

        public StructCityCoordinatesWrapper(IDataMapper mapper)
        {
            _mapper = mapper;
        }

        public string Name()
        {
            return "Coordinates";
        }

        public bool IsKey()
        {
            return false;
        }
        
        public object Value(object obj)
        {
            StructCity city = (StructCity) obj;
            return city.Coordinates;
        }
        
        public object GetValue(Dictionary<string, object> dictionary)
        {
            if(!dictionary.ContainsKey("Coordinates")) return null;
            return _mapper.GetById(dictionary["Coordinates"]);
        }

        public object AddValue(object obj)
        {
            StructCity city = (StructCity) obj;
            _mapper.Add(city.Coordinates);
            return city.Coordinates.Token;
        }

        public object UpdateValue(object obj)
        {
            StructCity city = (StructCity) obj;
            _mapper.Update(city.Coordinates);
            return city.Coordinates.Token;
        }

        public void DeleteValue(object keyValue, string collection)
        {
            if (keyValue is null) return;
            StructCoordinates value = (StructCoordinates) keyValue;
            if(collection.Equals(_mapper.Collection)) _mapper.Delete(value.Token);
        }
    }
}