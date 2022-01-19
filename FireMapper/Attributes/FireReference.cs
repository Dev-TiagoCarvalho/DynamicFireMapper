using System;

namespace FireMapper.Attributes
{
    public class FireReferenceAttribute : Attribute
    {
        private readonly string _collection;
        private readonly string _key;

        public FireReferenceAttribute(string collection, string key)
        {
            this._collection = collection;
            this._key = key;
        }

        public string GetCollection()
        {
            return this._collection;
        }

        public string GetKey()
        {
            return this._key;
        }
    }
}