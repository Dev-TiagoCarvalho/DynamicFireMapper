using System;
using System.Collections.Generic;
using System.Reflection;
using FireMapper.Attributes;
using FireMapper.Wrapper;
using FireSource;

namespace FireMapper
{
    public class FireDataMapper : BaseFireMapper
    { 
        public FireDataMapper(Type domain, Type source, string credentialsPath, string projectId)
        {
            FireCollectionAttribute collection = (FireCollectionAttribute) domain.GetCustomAttribute(typeof(FireCollectionAttribute));
            if(collection is null) throw new Exception($"Type {domain.Name} does not contain Attribute FireCollection");

            PropertyInfo[] properties = domain.GetProperties();
            String key = null;
            Type[] ctorParameters = new Type[properties.Length];
            IPropertyWrapper[] props = new IPropertyWrapper[properties.Length];
            
            for(int i = 0; i < properties.Length; ++i)
            {
                if(properties[i].IsDefined(typeof(FireIgnoreAttribute))) props[i] = null;
                else
                {
                    if(properties[i].IsDefined(typeof(FireKeyAttribute)))
                    {
                        if(key is null) key = properties[i].Name;
                        else throw new Exception($"Type {domain.Name} contains more than one Attribute FireKey");
                        KeyProperty = properties[i];
                    }
                    
                    Type propertyType = properties[i].PropertyType;
                    if(propertyType == typeof(string) || propertyType.IsPrimitive) props[i] = new PropertyPrimitive(properties[i], key);
                    else props[i] = new PropertyComplex(properties[i], key, new FireDataMapper(propertyType, source, credentialsPath, projectId));
                }
                
                ctorParameters[i] = properties[i].PropertyType;
            }
            if(key is null) throw new Exception($"Type {domain.Name} does not contain Attribute FireKey");
            
            ConstructorInfo ctor = domain.GetConstructor(ctorParameters);
            if(ctor is null) throw new Exception($"Type {domain.Name} does not contain a valid constructor");
            
            ConstructorInfo sourceCtor = source.GetConstructor(new [] {typeof(string), typeof(string), typeof(string), typeof(string)});
            if(sourceCtor is null) throw new Exception($"Type {source.Name} does not contain a valid constructor");

            Key = key;
            Collection = collection.GetCollection();
            Source = (IDataSource) sourceCtor.Invoke(new object[] { projectId, Collection, Key, credentialsPath });
            DomainProperties = props;
            DomainConstructor = ctor;
        }
    }
}