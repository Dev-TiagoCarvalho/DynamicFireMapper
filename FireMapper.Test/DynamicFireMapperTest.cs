using System;
using System.Linq;
using App.Data;
using FireSource;
using Xunit;

namespace FireMapper.Test
{
    [Collection("DATA_MAPPER_TESTS")]
    public class DynamicFireMapperTest : IDisposable
    {
        private static readonly string credentialPath = "../../../../App/Resources/";
        private static readonly string projectId = "offline_data_base.txt";

        public void Dispose()
        {
            WeakDataSource.ResetDataBase(projectId, credentialPath);
        }

        [Fact]
        public void GetAllTest()
        {
            Assert.Equal(3,new DynamicFireMapper(typeof(City), typeof(WeakDataSource), credentialPath, projectId).GetAll().Count());
            Assert.Equal(2,new DynamicFireMapper(typeof(VIP), typeof(WeakDataSource), credentialPath, projectId).GetAll().Count());
            Assert.Equal(3,new DynamicFireMapper(typeof(PointOfInterest), typeof(WeakDataSource), credentialPath, projectId).GetAll().Count());
        }

        [Fact]
        public void GetSpecificCityTest()
        {
            DynamicFireMapper mapper = new DynamicFireMapper(typeof(City), typeof(WeakDataSource), credentialPath, projectId);
            City city = (City) mapper.GetById("Lisboa");
            Assert.Equal("Lisboa",city.Name);
            Assert.Equal("Lisboa",city.Coordinates.Token);
            Assert.Equal(57.89,city.Coordinates.X, 4);
            Assert.Equal(-45.789,city.Coordinates.Y, 4);
            Assert.Equal("Portugal",city.Country);
            Assert.Equal(10000000,city.Population);
            Assert.Equal(59.7,city.Area, 2);
            Assert.Equal("GMT+1",city.TimeZone);
            Assert.Equal("Some description of Lisbon.",city.Description);
        }
        
        [Fact]
        public void GetSpecificVipTest()
        {
            DynamicFireMapper mapper = new DynamicFireMapper(typeof(VIP), typeof(WeakDataSource), credentialPath, projectId);
            VIP vip = ((VIP) mapper.GetById("Elon Musk"));
            Assert.Equal("Elon Musk",vip.Name);
            Assert.Equal("Lisboa",vip.City.Name);
            Assert.Equal("unemployed",vip.Job);
            Assert.Equal("01/01/2000",vip.Birthday);
            Assert.Equal("some description of Elon.",vip.Description);
        }
        
        [Fact]
        public void GetSpecificPointOfInterestTest()
        {
            DynamicFireMapper mapper = new DynamicFireMapper(typeof(PointOfInterest), typeof(WeakDataSource), credentialPath, projectId);
            PointOfInterest point = ((PointOfInterest) mapper.GetById("Capa Negra"));
            Assert.Equal("Capa Negra",point.Name);
            Assert.Equal("Porto",point.City.Name);
            Assert.Equal("Restaurant",point.Type);
            Assert.Equal(8,point.Evaluation);
            Assert.Equal("some description of Capa Negra.",point.Description);
        }
        
        [Fact]
        public void TryToGetExtinctTest()
        {
            DynamicFireMapper mapperCity = new DynamicFireMapper(typeof(City), typeof(WeakDataSource), credentialPath, projectId);
            City city = (City) mapperCity.GetById("Invalid City");
            Assert.Null(city);
            city = (City) mapperCity.GetById("Other Invalid City");
            Assert.Null(city);
            
            DynamicFireMapper mapperVip = new DynamicFireMapper(typeof(VIP), typeof(WeakDataSource), credentialPath, projectId);
            VIP vip = (VIP) mapperVip.GetById("Invalid VIP");
            Assert.Null(vip);
            
            DynamicFireMapper mapperPoi = new DynamicFireMapper(typeof(PointOfInterest), typeof(WeakDataSource), credentialPath, projectId);
            PointOfInterest poi = (PointOfInterest) mapperPoi.GetById("Invalid Point of Interest");
            Assert.Null(poi);
        }

        [Fact]
        public void AddCityTest()
        {
            DynamicFireMapper mapper = new DynamicFireMapper(typeof(City), typeof(WeakDataSource), credentialPath, projectId);
            Assert.Equal(3,mapper.GetAll().Count());
            mapper.Add(new City("Faro",new Coordinates("Faro",48.2222,-8.222),"Portugal",150000,5000,"GMT +1","Cidade de Faro",2));
            Assert.Equal(4,mapper.GetAll().Count());
        }
        
        [Fact]
        public void AddExistingCityTest()
        {
            City city = new City("Faro", new Coordinates("Faro", 48.2222, -8.222), "Portugal", 150000, 5000, "GMT +1", "Cidade de Faro", 2);
            DynamicFireMapper mapper = new DynamicFireMapper(typeof(City), typeof(WeakDataSource), credentialPath, projectId);
            Assert.Equal(3,mapper.GetAll().Count());
            mapper.Add(city);
            Assert.Equal(4,mapper.GetAll().Count());
            Assert.Throws<Exception>(() => mapper.Add(city));
        }

        [Fact]
        public void DeleteMultipleVipsTest()
        {
            DynamicFireMapper mapper = new DynamicFireMapper(typeof(VIP), typeof(WeakDataSource), credentialPath, projectId);
            Assert.Equal(2, mapper.GetAll().Count());
            mapper.Delete("Elon Musk");
            Assert.Single(mapper.GetAll());
            mapper.Delete("Vladimir Putin");
            Assert.Empty(mapper.GetAll());
        }
        
        [Fact]
        public void UpdateVipTest()
        {
            DynamicFireMapper mapperVip = new DynamicFireMapper(typeof(VIP), typeof(WeakDataSource), credentialPath, projectId);
            DynamicFireMapper mapperCity = new DynamicFireMapper(typeof(City), typeof(WeakDataSource), credentialPath, projectId);
            VIP elonUpdated = new VIP("Elon Musk", (City)mapperCity.GetById("Torres Vedras"), "employed", "02/02/2001", "Elon Updated");
            VIP elon = (VIP) mapperVip.GetById("Elon Musk");
            Assert.Equal("Lisboa", elon.City.Name);
            Assert.Equal("unemployed", elon.Job);
            Assert.Equal("01/01/2000", elon.Birthday);
            Assert.Equal("some description of Elon.", elon.Description);
            mapperVip.Update(elonUpdated);
            elon = (VIP) mapperVip.GetById("Elon Musk");
            Assert.Equal("Torres Vedras", elon.City.Name);
            Assert.Equal("employed", elon.Job);
            Assert.Equal("02/02/2001", elon.Birthday);
            Assert.Equal("Elon Updated", elon.Description);
        }
        
        [Fact]
        public void UpdateCityTest()
        {
            DynamicFireMapper mapper = new DynamicFireMapper(typeof(City), typeof(WeakDataSource), credentialPath, projectId);
            City city = new City("Porto",new Coordinates("Porto",-9.23,88.3),"Portugal",5000000,5000,"GMT+1","Some description of Porto",0);
            City cityUpdated = (City) mapper.GetById("Porto");
            Assert.Equal(88.3, cityUpdated.Coordinates.X,2);
            Assert.Equal(-9.23, cityUpdated.Coordinates.Y,3);
            Assert.Equal(20000000, cityUpdated.Population);
            Assert.Equal(90.7999, cityUpdated.Area, 5);
            mapper.Update(city);
            cityUpdated = (City) mapper.GetById("Porto");
            Assert.Equal(-9.23, cityUpdated.Coordinates.X,3);
            Assert.Equal(88.3, cityUpdated.Coordinates.Y,2);
            Assert.Equal(5000000, cityUpdated.Population);
            Assert.Equal(5000, cityUpdated.Area);
        }
        
        [Fact]
        public void UpdatePointOfInterestTest()
        {
            DynamicFireMapper mapper = new DynamicFireMapper(typeof(PointOfInterest), typeof(WeakDataSource), credentialPath, projectId);
            DynamicFireMapper mapperCity = new DynamicFireMapper(typeof(City), typeof(WeakDataSource), credentialPath, projectId);
            PointOfInterest point = new PointOfInterest("Padrão dos Descobrimentos",(City)mapperCity.GetById("Lisboa"),"Monument",5,"Padrão dos Descobrimentos is a monument");
            PointOfInterest pointUpdated = (PointOfInterest) mapper.GetById("Padrão dos Descobrimentos");
            Assert.Equal(9, pointUpdated.Evaluation);
            Assert.Equal("some description of Padrão dos Descobrimentos.", pointUpdated.Description);
            mapper.Update(point);
            pointUpdated = (PointOfInterest)mapper.GetById("Padrão dos Descobrimentos");
            Assert.Equal(5, pointUpdated.Evaluation);
            Assert.Equal("Padrão dos Descobrimentos is a monument", pointUpdated.Description);
        }
        
        [Fact]
        public void DeleteCityAndCoordinatesTest()
        {
            DynamicFireMapper mapperCity = new DynamicFireMapper(typeof(City), typeof(WeakDataSource), credentialPath, projectId);
            DynamicFireMapper mapperCords = new DynamicFireMapper(typeof(Coordinates), typeof(WeakDataSource), credentialPath, projectId);
            City porto = (City) mapperCity.GetById("Porto");
            Coordinates cords = (Coordinates) mapperCords.GetById("Porto");
            Assert.Equal(cords.Token, porto.Coordinates.Token);
            Assert.Equal(cords.X, porto.Coordinates.X);
            Assert.Equal(cords.Y, porto.Coordinates.Y);
            mapperCity.Delete("Porto");
            porto = (City) mapperCity.GetById("Porto");
            cords = (Coordinates) mapperCords.GetById("Porto");
            Assert.Null(porto);
            Assert.Null(cords);
        }
        
        [Fact]
        public void DeleteStudentAndClassroomTest()
        {
            DynamicFireMapper mapperStudent = new DynamicFireMapper(typeof(Student), typeof(WeakDataSource), credentialPath, projectId);
            DynamicFireMapper mapperClassroom = new DynamicFireMapper(typeof(Classroom), typeof(WeakDataSource), credentialPath, projectId);
            Student student = (Student) mapperStudent.GetById("Tiago");
            Classroom classroom = (Classroom) mapperClassroom.GetById(1);
            Assert.Equal(classroom.Id, student.Classroom.Id);
            Assert.Equal(classroom.Something, student.Classroom.Something);
            mapperStudent.Delete("Tiago");
            student = (Student) mapperStudent.GetById("Tiago");
            classroom = (Classroom) mapperClassroom.GetById(1);
            Assert.Null(student);
            Assert.Null(classroom);
        }
    }
}