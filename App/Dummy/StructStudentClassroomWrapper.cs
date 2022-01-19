using System.Collections.Generic;
using App.Data;
using FireMapper;
using FireMapper.Wrapper;

namespace App.Dummy
{
    public class StructStudentClassroomWrapper : IPropertyWrapper
    {
        private readonly IDataMapper _mapper;

        public StructStudentClassroomWrapper(IDataMapper mapper)
        {
            _mapper = mapper;
        }

        public string Name()
        {
            return "Classroom";
        }

        public bool IsKey()
        {
            return false;
        }
        
        public object Value(object obj)
        {
            StructStudent city = (StructStudent) obj;
            return city.Classroom;
        }

        public object GetValue(Dictionary<string, object> dictionary)
        {
            if(!dictionary.ContainsKey("Classroom")) return null;
            return _mapper.GetById(dictionary["Classroom"]);
        }

        public object AddValue(object obj)
        {
            StructStudent student = (StructStudent) obj;
            _mapper.Add(student.Classroom);
            return student.Classroom.Id;
        }

        public object UpdateValue(object obj)
        {
            StructStudent student = (StructStudent) obj;
            _mapper.Update(student.Classroom);
            return student.Classroom.Id;
        }

        public void DeleteValue(object keyValue, string collection)
        {
            if (keyValue is null) return;
            StructClassroom value = (StructClassroom) keyValue;
            if(collection.Equals(_mapper.Collection)) _mapper.Delete(value.Id);
        }
    }
}