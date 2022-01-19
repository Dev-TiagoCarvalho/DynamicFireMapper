using System;

namespace FireMapper.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class FireCollectionAttribute : Attribute
    {
        private readonly string _collection;

        public FireCollectionAttribute(string collection)
        {
            this._collection = collection;
        }

        public string GetCollection()
        {
            return this._collection;
        }
    }
}
