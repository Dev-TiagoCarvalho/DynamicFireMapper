using System;
using System.Collections.Generic;
using System.IO;
using Google.Cloud.Firestore;

namespace FireSource
{
    public class WeakDataSource : IDataSource
    {
        private readonly string _collection;
        private readonly string _key;

        public string ProjectId { get; set; }
        public string Collection { get; set; }
        public string Key { get; set; }
        public string CredentialsPath { get; set; }

        private readonly string _path;
        
        private static readonly Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, object>>>> DataBase = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, object>>>>();

        public WeakDataSource(string projectId, string collection, string key, string credentialsPath)
        {
            _collection = collection;
            _key = key;
            _path = credentialsPath + projectId;
            if(!DataBase.ContainsKey(_path))
            {
                DataBase[_path] = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();
                PopulateDataBase(_path);
            }
        }

        private static void PopulateDataBase(string path)
        {
            String[] lines = File.ReadAllLines(path);
            foreach(String line in lines)
            {
                if(line[0] == '/' && line[1] == '/') continue;
                String[] split = line.Split(';');
                if(split.Length <= 2) continue;
                if(!DataBase[path].ContainsKey(split[0])) DataBase[path][split[0]] = new Dictionary<string, Dictionary<string, object>>();
                if(!DataBase[path][split[0]].ContainsKey(split[1])) DataBase[path][split[0]][split[1]] = new Dictionary<string, object>();
                for(int i = 2; i < split.Length; ++i)
                {
                    String[] pair = split[i].Split(':');
                    if(pair.Length != 2) continue;
                    if(DataBase[path][split[0]][split[1]].ContainsKey(pair[0])) continue;
                    if(long.TryParse(pair[1], out var int64)) DataBase[path][split[0]][split[1]].Add(pair[0], int64);
                    else if(double.TryParse(pair[1], out var float64)) DataBase[path][split[0]][split[1]].Add(pair[0], float64);
                    else DataBase[path][split[0]][split[1]].Add(pair[0], pair[1]);
                }
            }
        }

        public static void ResetDataBase(string file, string path)
        {
            string full = path + file;
            DataBase.Remove(full);
            DataBase[full] = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();
            PopulateDataBase(full);
        }

        public IEnumerable<Dictionary<string, object>> GetAll()
        {
            List<Dictionary<string, object>> toReturn = new List<Dictionary<string, object>>();
            foreach(var document in DataBase[this._path][this._collection])
            {
                toReturn.Add(document.Value);
            }
            return toReturn;
        }

        public Dictionary<string, object> GetById(object keyValue)
        {
            foreach(var document in DataBase[this._path][this._collection])
            {
                try
                {
                    if (document.Value.ContainsKey(this._key))
                    {
                        long l1 = (long) Convert.ChangeType(document.Value[this._key], typeof(long));
                        long l2 = (long) Convert.ChangeType(keyValue, typeof(long));
                        if (l1 == l2) return document.Value;
                    }
                    
                }
                catch (Exception)
                {
                    if(document.Value.ContainsKey(this._key) && document.Value[this._key].Equals(keyValue)) return document.Value;
                }
            }
            return null;
        }

        public void Add(Dictionary<string, object> obj)
        {
            Random random = new Random();
            string reference;
            while(true)
            {
                reference = random.Next().ToString();
                if(!DataBase[this._path][this._collection].ContainsKey(reference)) break;
            }
            DataBase[this._path][this._collection][reference] = obj;
        }

        public void Update(Dictionary<string, object> obj)
        {
            if(!obj.ContainsKey(this._key)) return;
            foreach(var document in DataBase[this._path][this._collection])
            {
                if(document.Value.ContainsKey(this._key) && document.Value[this._key].Equals(obj[this._key]))
                {
                    DataBase[this._path][this._collection][document.Key] = obj;
                    return;
                }
            }
        }

        public void Delete(object keyValue)
        {
            foreach(var document in DataBase[this._path][this._collection])
            {
                try
                {
                    if (document.Value.ContainsKey(this._key))
                    {
                        long l1 = (long) Convert.ChangeType(document.Value[this._key], typeof(long));
                        long l2 = (long) Convert.ChangeType(keyValue, typeof(long));
                        if (l1 == l2)
                        {
                            DataBase[this._path][this._collection].Remove(document.Key);
                            return;
                        }
                    }
                    
                }
                catch (Exception)
                {
                    if (document.Value.ContainsKey(this._key) && document.Value[this._key].Equals(keyValue))
                    {
                        DataBase[this._path][this._collection].Remove(document.Key);
                        return;
                    }
                }
            }
        }
    }
}