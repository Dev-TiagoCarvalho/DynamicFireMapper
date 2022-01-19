using System;
using System.Collections.Generic;
using System.Reflection;
using FireMapper.Attributes;
using FireMapper.Wrapper;
using FireSource;

namespace FireMapper
{
    public class DynamicFireMapper : BaseFireMapper
    {
        public DynamicFireMapper(Type domain, Type source, string credentialsPath, string projectId)
        {
            FireCollectionAttribute collection = (FireCollectionAttribute) domain.GetCustomAttribute(typeof(FireCollectionAttribute));
            if(collection is null) throw new Exception($"Type {domain.Name} does not contain Attribute FireCollection");

            PropertyInfo[] properties = domain.GetProperties();
            String key = null;
            Type[] ctorParameters = new Type[properties.Length];
            IPropertyWrapper[] props = new IPropertyWrapper[properties.Length];
            WrapperBuilder builder = new WrapperBuilder(domain);

            for(int i = 0; i < properties.Length; ++i)
            {
                if(properties[i].IsDefined(typeof(FireIgnoreAttribute))) props[i] = null;
                else
                {
                    Type propertyType = properties[i].PropertyType;
                    Type wrapper = builder.GenerateFor(properties[i]);
                    if(propertyType == typeof(string) || propertyType.IsPrimitive) props[i] = (IPropertyWrapper) Activator.CreateInstance(wrapper);
                    else
                    {
                        ConstructorInfo wrapperCtor = wrapper.GetConstructor(new []{typeof(IDataMapper)});
                        if (wrapperCtor is null) throw new Exception("Couldn't generate dynamic constructor of IPropertyWrapper");
                        object[] array = { new DynamicFireMapper(propertyType, source, credentialsPath, projectId) };
                        props[i] = (IPropertyWrapper) wrapperCtor.Invoke(array);
                    }
                    
                    if(properties[i].IsDefined(typeof(FireKeyAttribute)))
                    {
                        if(key is null) key = properties[i].Name;
                        else throw new Exception($"Type {domain.Name} contains more than one Attribute FireKey");

                        KeyProperty = properties[i];
                    }
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
            Domain = domain;
            IsValueType = typeof(ValueType).IsAssignableFrom(domain);
            Source = (IDataSource) sourceCtor.Invoke(new object[] { projectId, Collection, Key, credentialsPath });
            DomainProperties = props;
            DomainConstructor = ctor;
        }
    }
}