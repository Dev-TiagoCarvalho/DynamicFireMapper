using System;
using System.Collections.Generic;
using System.IO;

namespace FireSource
{
    public class NonExecutableSource : IDataSource
    {
        private readonly string _key;
        private readonly string _collection;
        private readonly string _path;
        
        private static readonly Dictionary<string, Dictionary<string, object>> DataBase = new Dictionary<string, Dictionary<string, object>>();
        
        public NonExecutableSource(string projectId, string collection, string key, string credentialsPath)
        {
            _key = key;
            _collection = collection;
            _path = credentialsPath + projectId;
            if(!DataBase.ContainsKey(key))
            {
                DataBase[key] = new Dictionary<string, object>();
                PopulateDataBase(key);
            }
        }
        
        private static void PopulateDataBase(string key)
        {
            DataBase[key]["Name"] = "Lisboa";
            DataBase[key]["Coordinates"] = "Lisboa";
            DataBase[key]["Country"] = "Portugal";
            DataBase[key]["Population"] = 10000L;
            DataBase[key]["Area"] = 67.9;
            DataBase[key]["TimeZone"] = "UTC+0";
            DataBase[key]["Description"] = "Some Description";
            DataBase[key]["Token"] = "Lisboa";
            DataBase[key]["X"] = 45.9;
            DataBase[key]["Y"] = -45.9;
        }
        
        public IEnumerable<Dictionary<string, object>> GetAll()
        {
            List<Dictionary<string, object>> toReturn = new List<Dictionary<string, object>>();
            toReturn.Add(DataBase[_key]);
            return toReturn;
        }

        public Dictionary<string, object> GetById(object keyValue)
        {
            if(DataBase[_key]["Name"].Equals(keyValue)) return DataBase[_key];
            if(DataBase[_key]["Token"].Equals(keyValue)) return DataBase[_key];
            return null;
        }

        public void Add(Dictionary<string, object> obj)
        {
            
        }

        public void Update(Dictionary<string, object> obj)
        {
            
        }

        public void Delete(object keyValue)
        {
            
        }
    }
}