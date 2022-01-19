using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using FireMapper.Attributes;

namespace FireMapper.Wrapper
{
    public class WrapperBuilder
    {
        private readonly AssemblyBuilder _builder;
        private readonly ModuleBuilder _module;
        private readonly Type _domain;
        private readonly AssemblyName _assemblyName;

        private readonly MethodInfo _equals;
        private readonly MethodInfo _containsKey;
        private readonly MethodInfo _getItem;
        private readonly MethodInfo _add;
        private readonly MethodInfo _update;
        private readonly MethodInfo _delete;
        private readonly MethodInfo _getById;
        private readonly MethodInfo _getCollection;
        private readonly MethodInfo _typeof;
        private readonly MethodInfo _changeType;
        private readonly MethodInfo _value;
        private readonly ConstructorInfo _baseCtor;

        private static readonly Dictionary<Type, PropertyInfo> Cache = new Dictionary<Type, PropertyInfo>();
        
        public WrapperBuilder(Type domain)
        {
            _baseCtor = typeof(object).GetConstructor(Type.EmptyTypes);
            if (_baseCtor is null) throw new Exception("Couldn't load base constructor from object");

            _equals = typeof(String).GetMethod("Equals", new []{typeof(string)});
            if (_equals is null) throw new Exception("Couldn't load method Equals(string) from string");

            Type dictionaryType = typeof(Dictionary<string, object>);
            _containsKey = dictionaryType.GetMethod("ContainsKey");
            _getItem = dictionaryType.GetMethod("get_Item");
            if (_containsKey is null || _getItem is null) throw new Exception("Couldn't load methods from Dictionary<string, object>");

            Type mapperType = typeof(IDataMapper);
            _update = mapperType.GetMethod("Update");
            _add = mapperType.GetMethod("Add");
            _getById = mapperType.GetMethod("GetById");
            _getCollection = mapperType.GetMethod("get_Collection");
            _delete = mapperType.GetMethod("Delete");
            if (_update is null || _add is null || _getById is null || _getCollection is null || _delete is null) throw new Exception("Couldn't load methods from IDataMapper");

            _value = typeof(IPropertyWrapper).GetMethod("Value");
            if (_value is null) throw new Exception("Couldn't load methods from IPropertyWrapper");

            _typeof = typeof(Type).GetMethod("GetTypeFromHandle");
            _changeType = typeof(Convert).GetMethod("ChangeType", new []{typeof(object), typeof(Type)});
            if (_typeof is null || _changeType is null) throw new Exception("Couldn't load methods from Convert");
            
            _domain = domain;
            _assemblyName = new AssemblyName(domain.Name + "Wrappers");
            _builder = AssemblyBuilder.DefineDynamicAssembly(_assemblyName, AssemblyBuilderAccess.RunAndSave);
            _module = _builder.DefineDynamicModule(_assemblyName.Name, _assemblyName.Name + ".dll");
        }

        public void SaveModule()
        {
            _builder.Save(_assemblyName.Name + "dll");
        }

        public Type GenerateFor(PropertyInfo property)
        {
            Type propertyType = property.PropertyType;
            if(propertyType == typeof(string) || propertyType.IsPrimitive) return GeneratePrimitive(property);
            return GenerateComplex(property);
        }

        private TypeBuilder GenerateType(PropertyInfo property)
        {
            TypeBuilder type = _module.DefineType($"{_domain.Name}{property.Name}Wrapper", TypeAttributes.Public | TypeAttributes.Class, null, new []{typeof(IPropertyWrapper)});
            GenerateGetName(type, property.Name);
            GenerateIsKey(type, property);
            GenerateValue(type, property);
            return type;
        }

        private void GenerateGetName(TypeBuilder type, string name)
        {
            MethodBuilder method = type.DefineMethod("Name", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Final, typeof(string), Type.EmptyTypes);
            ILGenerator il = method.GetILGenerator();
            il.Emit(OpCodes.Ldstr, name);
            il.Emit(OpCodes.Ret);
        }

        private void GenerateIsKey(TypeBuilder type, PropertyInfo property)
        {
            MethodBuilder method = type.DefineMethod("IsKey", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Final, typeof(bool), Type.EmptyTypes);
            ILGenerator il = method.GetILGenerator();
            il.Emit(Attribute.IsDefined(property, typeof(FireKeyAttribute)) ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Ret);
        }
        
        private void GenerateValue(TypeBuilder type, PropertyInfo property)
        {
            MethodBuilder method = type.DefineMethod("Value", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Final, typeof(object), new [] {typeof(object)});
            ILGenerator il = method.GetILGenerator();
            il.Emit(OpCodes.Ldarg_1);
            if (typeof(ValueType).IsAssignableFrom(_domain))
            {
                LocalBuilder local = il.DeclareLocal(_domain);
                il.Emit(OpCodes.Unbox_Any, _domain);
                il.Emit(OpCodes.Stloc_0, local);
                il.Emit(OpCodes.Ldloca_S, local);
                il.Emit(OpCodes.Call, property.GetMethod);
            }
            else
            {
                il.Emit(OpCodes.Castclass, _domain);
                il.Emit(OpCodes.Callvirt, property.GetMethod);
            }
            if(property.PropertyType.IsPrimitive || typeof(ValueType).IsAssignableFrom(property.PropertyType)) il.Emit(OpCodes.Box, property.PropertyType);
            il.Emit(OpCodes.Ret);
        }
        
        private Type GeneratePrimitive(PropertyInfo property)
        {
            TypeBuilder typeBuilder = GenerateType(property);
            GeneratePrimitiveConstructor(typeBuilder);
            GeneratePrimitiveGetValue(typeBuilder, property);
            GeneratePrimitiveSetValue("AddValue", typeBuilder);
            GeneratePrimitiveSetValue("UpdateValue", typeBuilder);
            GeneratePrimitiveDeleteValue(typeBuilder);
            return typeBuilder.CreateType();
        }
        
        private void GeneratePrimitiveConstructor(TypeBuilder type)
        {
            ConstructorBuilder ctor = type.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
            ILGenerator il = ctor.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, _baseCtor);
            il.Emit(OpCodes.Ret);
        }
        
        private void GeneratePrimitiveGetValue(TypeBuilder type, PropertyInfo property)
        {
            MethodBuilder method = type.DefineMethod("GetValue", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Final, typeof(object), new[] {typeof(Dictionary<string, object>)});
            ILGenerator il = method.GetILGenerator();
            Label label = il.DefineLabel();
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldstr, property.Name);
            il.Emit(OpCodes.Callvirt, _containsKey);
            il.Emit(OpCodes.Brtrue_S, label);
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Ret);
            il.MarkLabel(label);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldstr, property.Name);
            il.Emit(OpCodes.Callvirt, _getItem);
            if (property.PropertyType.IsPrimitive)
            {
                il.Emit(OpCodes.Ldtoken, property.PropertyType);
                il.Emit(OpCodes.Call, _typeof);
                il.Emit(OpCodes.Call, _changeType);
            }
            il.Emit(OpCodes.Ret);
        }

        private void GeneratePrimitiveSetValue(string name, TypeBuilder type)
        {
            MethodBuilder method = type.DefineMethod(name, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Final, typeof(object), new[] {typeof(object)});
            ILGenerator il = method.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, _value);
            il.Emit(OpCodes.Ret);
        }

        private void GeneratePrimitiveDeleteValue(TypeBuilder type)
        {
            MethodBuilder method = type.DefineMethod("DeleteValue", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Final, null, new[] {typeof(object), typeof(string)});
            ILGenerator il = method.GetILGenerator();
            il.Emit(OpCodes.Ret);
        }

        private Type GenerateComplex(PropertyInfo property)
        {
            TypeBuilder typeBuilder = GenerateType(property);
            FieldBuilder fieldBuilder = typeBuilder.DefineField("mapper", typeof(IDataMapper), FieldAttributes.Private);
            GenerateComplexConstructor(typeBuilder, fieldBuilder);
            GenerateComplexGetValue(typeBuilder, fieldBuilder, property.Name);
            GenerateComplexSetValue("AddValue", typeBuilder, fieldBuilder, _add, property);
            GenerateComplexSetValue("UpdateValue", typeBuilder, fieldBuilder, _update, property);
            GenerateComplexDeleteValue(typeBuilder, fieldBuilder, property);
            return typeBuilder.CreateType();
        }

        private void GenerateComplexConstructor(TypeBuilder type, FieldInfo field)
        {
            ConstructorBuilder ctor = type.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new []{typeof(IDataMapper)});
            ILGenerator il = ctor.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, _baseCtor);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, field);
            il.Emit(OpCodes.Ret);
        }

        private void GenerateComplexGetValue(TypeBuilder type, FieldInfo field, string name)
        {
            MethodBuilder method = type.DefineMethod("GetValue", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Final, typeof(object), new[] {typeof(Dictionary<string, object>)});
            ILGenerator il = method.GetILGenerator();
            Label label = il.DefineLabel();
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldstr, name);
            il.Emit(OpCodes.Callvirt, _containsKey);
            il.Emit(OpCodes.Brtrue_S, label);
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Ret);
            il.MarkLabel(label);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, field);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldstr, name);
            il.Emit(OpCodes.Callvirt, _getItem);
            il.Emit(OpCodes.Callvirt, _getById);
            il.Emit(OpCodes.Ret);
        }

        private void GenerateComplexSetValue(string methodName, TypeBuilder type, FieldInfo field, MethodInfo methodInfo, PropertyInfo property)
        {
            bool isStruct = typeof(ValueType).IsAssignableFrom(_domain);
            PropertyInfo nextProperty = GetKeyProperty(property.PropertyType);
            MethodBuilder method = type.DefineMethod(methodName, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Final, typeof(object), new[] {typeof(object)});
            ILGenerator il = method.GetILGenerator();
            LocalBuilder local0 = il.DeclareLocal(_domain);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(isStruct ? OpCodes.Unbox_Any : OpCodes.Castclass, _domain);
            il.Emit(OpCodes.Stloc_0, local0);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, field);
            if (isStruct)
            {
                il.Emit(OpCodes.Ldloca_S, local0);
                il.Emit(OpCodes.Call, property.GetMethod);
                il.Emit(OpCodes.Box, property.PropertyType);
            }
            else
            {
                il.Emit(OpCodes.Ldloc_0, local0);
                il.Emit(OpCodes.Callvirt, property.GetMethod);
            }
            il.Emit(OpCodes.Callvirt, methodInfo);
            if (isStruct)
            {
                LocalBuilder local1 = il.DeclareLocal(property.PropertyType);
                il.Emit(OpCodes.Ldloca_S, local0);
                il.Emit(OpCodes.Call, property.GetMethod);
                il.Emit(OpCodes.Stloc_1, local1);
                il.Emit(OpCodes.Ldloca_S, local1);
                il.Emit(OpCodes.Call, nextProperty.GetMethod);
            }
            else
            {
                il.Emit(OpCodes.Ldloc_0, local0);
                il.Emit(OpCodes.Callvirt, property.GetMethod);
                il.Emit(OpCodes.Callvirt, nextProperty.GetMethod);
            }
            if(nextProperty.PropertyType.IsPrimitive) il.Emit(OpCodes.Box, nextProperty.PropertyType);
            il.Emit(OpCodes.Ret);
        }

        private void GenerateComplexDeleteValue(TypeBuilder type, FieldInfo field, PropertyInfo property)
        {
            bool isStruct = typeof(ValueType).IsAssignableFrom(_domain);
            PropertyInfo nextProperty = GetKeyProperty(property.PropertyType);
            MethodBuilder method = type.DefineMethod("DeleteValue", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Final, null, new[] {typeof(object), typeof(string)});
            ILGenerator il = method.GetILGenerator();
            LocalBuilder local = il.DeclareLocal(property.PropertyType);
            Label label0 = il.DefineLabel();
            Label label1 = il.DefineLabel();
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Brtrue_S, label0);
            il.Emit(OpCodes.Ret);
            il.MarkLabel(label0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(isStruct ? OpCodes.Unbox_Any : OpCodes.Castclass, property.PropertyType);
            il.Emit(OpCodes.Stloc_0, local);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, field);
            il.Emit(OpCodes.Callvirt, _getCollection);
            il.Emit(OpCodes.Callvirt, _equals);
            il.Emit(OpCodes.Brfalse_S, label1);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, field);
            if (isStruct)
            {
                il.Emit(OpCodes.Ldloca_S, local);
                il.Emit(OpCodes.Call, nextProperty.GetMethod);
            }
            else
            {
                il.Emit(OpCodes.Ldloc_0, local);
                il.Emit(OpCodes.Callvirt, nextProperty.GetMethod);
            }
            if(nextProperty.PropertyType.IsPrimitive) il.Emit(OpCodes.Box, nextProperty.PropertyType);
            il.Emit(OpCodes.Callvirt, _delete);
            il.MarkLabel(label1);
            il.Emit(OpCodes.Ret);
        }

        private PropertyInfo GetKeyProperty(Type type)
        {
            if (Cache.ContainsKey(type)) return Cache[type];
            foreach (PropertyInfo property in type.GetProperties())
            {
                if (Attribute.IsDefined(property, typeof(FireKeyAttribute)))
                {
                    Cache[type] = property;
                    return property;
                }
            }

            throw new Exception($"Type {type.Name} does not contain Attribute FireKey");
        }
    }
}