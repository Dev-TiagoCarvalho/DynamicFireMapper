using System.Collections.Generic;
using App.Data;
using FireMapper;
using FireMapper.Wrapper;

namespace App.Dummy
{
    public class StudentClassroomWrapper : IPropertyWrapper
    {
        private readonly IDataMapper _mapper;

        public StudentClassroomWrapper(IDataMapper mapper)
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
            Student city = (Student) obj;
            return city.Classroom;
        }

        public object GetValue(Dictionary<string, object> dictionary)
        {
            if(!dictionary.ContainsKey("Classroom")) return null;
            return _mapper.GetById(dictionary["Classroom"]);
        }

        public object AddValue(object obj)
        {
            Student student = (Student) obj;
            _mapper.Add(student.Classroom);
            return student.Classroom.Id;
        }

        public object UpdateValue(object obj)
        {
            Student student = (Student) obj;
            _mapper.Update(student.Classroom);
            return student.Classroom.Id;
        }

        public void DeleteValue(object keyValue, string collection)
        {
            if (keyValue is null) return;
            Classroom value = (Classroom) keyValue;
            if(collection.Equals(_mapper.Collection)) _mapper.Delete(value.Id);
        }
    }
}