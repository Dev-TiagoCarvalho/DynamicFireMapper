using System;
using App.Data;
using FireMapper;
using FireSource;

namespace Benchmark
{
    public static class Program
    {
        private static readonly string credentialPath = "../../../../App/Resources/";
        private static readonly string projectId = "offline_data_base.txt";
        
        public static void Main(string[] args)
        {
            FireDataMapper mapperCity = new FireDataMapper(typeof(City), typeof(NonExecutableSource), credentialPath, projectId);
            DynamicFireMapper dynamicMapperCity = new DynamicFireMapper(typeof(City), typeof(NonExecutableSource), credentialPath, projectId); 
            City city = new City("Faro", new Coordinates("Faro", 48.2222, -8.222), "Portugal", 150000, 5000, "GMT +1", "Cidade de Faro", 2);
            
            PrintBestPerformance(Benchmark.Bench(() => { mapperCity.GetById("Lisboa"); }), Benchmark.Bench(() => { dynamicMapperCity.GetById("Lisboa"); }));
            PrintBestPerformance(Benchmark.Bench(() => { mapperCity.Update(city); }), Benchmark.Bench(() => { dynamicMapperCity.Update(city); }));
            PrintBestPerformance(Benchmark.Bench(() => { mapperCity.Delete("Lisboa"); }), Benchmark.Bench(() => { dynamicMapperCity.Delete("Lisboa"); }));
        }

        private static void PrintBestPerformance(long reflection, long dynamic)
        {
            if(reflection > dynamic) Console.WriteLine($"BEST PERFORMANCE: FireDataMapper > {(double)(reflection - dynamic) / reflection * 100}%");
            else if(reflection < dynamic) Console.WriteLine($"BEST PERFORMANCE: DynamicDataMapper > {(double)(dynamic - reflection) / dynamic * 100}%");
            else if(reflection == dynamic) Console.WriteLine("SAME PERFORMANCE IN BOTH FireDataMapper AND DynamicDataMapper");
            Console.WriteLine();
        }
    }
}
