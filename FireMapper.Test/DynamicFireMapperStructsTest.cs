using System;
using System.Linq;
using App.Data;
using FireSource;
using Xunit;

namespace FireMapper.Test
{
    [Collection("DATA_MAPPER_TESTS")]
    public class DynamicFireMapperStructsTest : IDisposable
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
            Assert.Equal(3,new DynamicFireMapper(typeof(StructCity), typeof(WeakDataSource), credentialPath, projectId).GetAll().Count());
            Assert.Equal(2,new DynamicFireMapper(typeof(StructVIP), typeof(WeakDataSource), credentialPath, projectId).GetAll().Count());
            Assert.Equal(3,new DynamicFireMapper(typeof(StructPointOfInterest), typeof(WeakDataSource), credentialPath, projectId).GetAll().Count());
        }

        [Fact]
        public void GetSpecificCityTest()
        {
            DynamicFireMapper mapper = new DynamicFireMapper(typeof(StructCity), typeof(WeakDataSource), credentialPath, projectId);
            StructCity structCity = (StructCity) mapper.GetById("Lisboa");
            Assert.Equal("Lisboa",structCity.Name);
            Assert.Equal("Lisboa",structCity.Coordinates.Token);
            Assert.Equal(57.89,structCity.Coordinates.X, 4);
            Assert.Equal(-45.789,structCity.Coordinates.Y, 4);
            Assert.Equal("Portugal",structCity.Country);
            Assert.Equal(10000000,structCity.Population);
            Assert.Equal(59.7,structCity.Area, 2);
            Assert.Equal("GMT+1",structCity.TimeZone);
            Assert.Equal("Some description of Lisbon.",structCity.Description);
        }
        
        [Fact]
        public void GetSpecificVipTest()
        {
            DynamicFireMapper mapper = new DynamicFireMapper(typeof(StructVIP), typeof(WeakDataSource), credentialPath, projectId);
            StructVIP vip = (StructVIP) mapper.GetById("Elon Musk");
            Assert.Equal("Elon Musk",vip.Name);
            Assert.Equal("Lisboa",vip.City.Name);
            Assert.Equal("unemployed",vip.Job);
            Assert.Equal("01/01/2000",vip.Birthday);
            Assert.Equal("some description of Elon.",vip.Description);
        }
        
        [Fact]
        public void GetSpecificPointOfInterestTest()
        {
            DynamicFireMapper mapper = new DynamicFireMapper(typeof(StructPointOfInterest), typeof(WeakDataSource), credentialPath, projectId);
            StructPointOfInterest point = (StructPointOfInterest) mapper.GetById("Capa Negra");
            Assert.Equal("Capa Negra",point.Name);
            Assert.Equal("Porto",point.City.Name);
            Assert.Equal("Restaurant",point.Type);
            Assert.Equal(8,point.Evaluation);
            Assert.Equal("some description of Capa Negra.",point.Description);
        }
        
        [Fact]
        public void TryToGetExtinctTest()
        {
            DynamicFireMapper mapperStructCity = new DynamicFireMapper(typeof(StructCity), typeof(WeakDataSource), credentialPath, projectId);
            StructCity structCity = (StructCity) mapperStructCity.GetById("Invalid StructCity");
            Assert.Null(structCity.Name);
            structCity = (StructCity) mapperStructCity.GetById("Other Invalid StructCity");
            Assert.Null(structCity.Name);
            
            DynamicFireMapper mapperVip = new DynamicFireMapper(typeof(StructVIP), typeof(WeakDataSource), credentialPath, projectId);
            StructVIP structVip = (StructVIP) mapperVip.GetById("Invalid VIP");
            Assert.Null(structVip.Name);
            
            DynamicFireMapper mapperPoi = new DynamicFireMapper(typeof(StructPointOfInterest), typeof(WeakDataSource), credentialPath, projectId);
            StructPointOfInterest structPointOfInterest = (StructPointOfInterest) mapperPoi.GetById("Invalid Point of Interest");
            Assert.Null(structPointOfInterest.Name);
        }

        [Fact]
        public void AddStructCityTest()
        {
            DynamicFireMapper mapper = new DynamicFireMapper(typeof(StructCity), typeof(WeakDataSource), credentialPath, projectId);
            Assert.Equal(3,mapper.GetAll().Count());
            mapper.Add(new StructCity("Faro",new StructCoordinates("Faro",48.2222,-8.222),"Portugal",150000,5000,"GMT +1","Cidade de Faro",2));
            Assert.Equal(4,mapper.GetAll().Count());
        }
        
        [Fact]
        public void AddExistingStructCityTest()
        {
            StructCity structCity = new StructCity("Faro", new StructCoordinates("Faro", 48.2222, -8.222), "Portugal", 150000, 5000, "GMT +1", "Cidade de Faro", 2);
            DynamicFireMapper mapper = new DynamicFireMapper(typeof(StructCity), typeof(WeakDataSource), credentialPath, projectId);
            Assert.Equal(3,mapper.GetAll().Count());
            mapper.Add(structCity);
            Assert.Equal(4,mapper.GetAll().Count());
            Assert.Throws<Exception>(() => mapper.Add(structCity));
        }

        [Fact]
        public void DeleteMultipleVipsTest()
        {
            DynamicFireMapper mapper = new DynamicFireMapper(typeof(StructVIP), typeof(WeakDataSource), credentialPath, projectId);
            Assert.Equal(2, mapper.GetAll().Count());
            mapper.Delete("Elon Musk");
            Assert.Single(mapper.GetAll());
            mapper.Delete("Vladimir Putin");
            Assert.Empty(mapper.GetAll());
        }
        
        [Fact]
        public void UpdateVipTest()
        {
            DynamicFireMapper mapperVip = new DynamicFireMapper(typeof(StructVIP), typeof(WeakDataSource), credentialPath, projectId);
            DynamicFireMapper mapperStructCity = new DynamicFireMapper(typeof(StructCity), typeof(WeakDataSource), credentialPath, projectId);
            StructVIP elonUpdated = new StructVIP("Elon Musk", (StructCity) mapperStructCity.GetById("Torres Vedras"), "employed", "02/02/2001", "Elon Updated");
            StructVIP elon = (StructVIP) mapperVip.GetById("Elon Musk");
            Assert.Equal("Lisboa", elon.City.Name);
            Assert.Equal("unemployed", elon.Job);
            Assert.Equal("01/01/2000", elon.Birthday);
            Assert.Equal("some description of Elon.", elon.Description);
            mapperVip.Update(elonUpdated);
            elon = (StructVIP) mapperVip.GetById("Elon Musk");
            Assert.Equal("Torres Vedras", elon.City.Name);
            Assert.Equal("employed", elon.Job);
            Assert.Equal("02/02/2001", elon.Birthday);
            Assert.Equal("Elon Updated", elon.Description);
        }
        
        [Fact]
        public void UpdateStructCityTest()
        {
            DynamicFireMapper mapper = new DynamicFireMapper(typeof(StructCity), typeof(WeakDataSource), credentialPath, projectId);
            StructCity structCity = new StructCity("Porto",new StructCoordinates("Porto",-9.23,88.3),"Portugal",5000000,5000,"GMT+1","Some description of Porto",0);
            StructCity structCityUpdated = (StructCity) mapper.GetById("Porto");
            Assert.Equal(88.3, structCityUpdated.Coordinates.X,2);
            Assert.Equal(-9.23, structCityUpdated.Coordinates.Y,3);
            Assert.Equal(20000000, structCityUpdated.Population);
            Assert.Equal(90.7999, structCityUpdated.Area, 5);
            mapper.Update(structCity);
            structCityUpdated = (StructCity) mapper.GetById("Porto");
            Assert.Equal(-9.23, structCityUpdated.Coordinates.X,3);
            Assert.Equal(88.3, structCityUpdated.Coordinates.Y,2);
            Assert.Equal(5000000, structCityUpdated.Population);
            Assert.Equal(5000, structCityUpdated.Area);
        }
        
        [Fact]
        public void UpdatePointOfInterestTest()
        {
            DynamicFireMapper mapper = new DynamicFireMapper(typeof(StructPointOfInterest), typeof(WeakDataSource), credentialPath, projectId);
            DynamicFireMapper mapperStructCity = new DynamicFireMapper(typeof(StructCity), typeof(WeakDataSource), credentialPath, projectId);
            StructPointOfInterest point = new StructPointOfInterest("Padrão dos Descobrimentos",(StructCity) mapperStructCity.GetById("Lisboa"),"Monument",5,"Padrão dos Descobrimentos is a monument");
            StructPointOfInterest pointUpdated = (StructPointOfInterest) mapper.GetById("Padrão dos Descobrimentos");
            Assert.Equal(9, pointUpdated.Evaluation);
            Assert.Equal("some description of Padrão dos Descobrimentos.", pointUpdated.Description);
            mapper.Update(point);
            pointUpdated = (StructPointOfInterest)mapper.GetById("Padrão dos Descobrimentos");
            Assert.Equal(5, pointUpdated.Evaluation);
            Assert.Equal("Padrão dos Descobrimentos is a monument", pointUpdated.Description);
        }
        
        [Fact]
        public void DeleteStructCityAndCoordinatesTest()
        {
            DynamicFireMapper mapperStructCity = new DynamicFireMapper(typeof(StructCity), typeof(WeakDataSource), credentialPath, projectId);
            DynamicFireMapper mapperCords = new DynamicFireMapper(typeof(StructCoordinates), typeof(WeakDataSource), credentialPath, projectId);
            StructCity porto = (StructCity) mapperStructCity.GetById("Porto");
            StructCoordinates cords = (StructCoordinates) mapperCords.GetById("Porto");
            Assert.Equal(cords.Token, porto.Coordinates.Token);
            Assert.Equal(cords.X, porto.Coordinates.X);
            Assert.Equal(cords.Y, porto.Coordinates.Y);
            mapperStructCity.Delete("Porto");
            porto = (StructCity) mapperStructCity.GetById("Porto");
            cords = (StructCoordinates) mapperCords.GetById("Porto");
            Assert.Null(porto.Name);
            Assert.Null(cords.Token);
        }
    }
}