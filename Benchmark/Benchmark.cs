using System;

namespace Benchmark
{
    public static class Benchmark
    {
        /**
         * Original Source:
         * https://github.com/isel-leic-ave/ave-2019-20-sem2-i41d/blob/master/aula21-benchmarking/NBench.cs
         */
        public static long Bench(Action handler)
        {
            Console.WriteLine("BENCHMARKING: START");
            long opsPerMSec = Perform(handler, 1000, 10);
            Console.WriteLine($"BEST: {opsPerMSec} operations/millisecond");
            return opsPerMSec;
        }

        private static void CleanGarbageCollector()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private static long Perform(Action handler, long millis, int iterations)
        {
            CleanGarbageCollector();
            long bestPerform = 0;

            for (int i = 0; i < iterations; i++)
            {
                long opsPerMSec = Perform32Times(handler, millis);
                Console.WriteLine($"ITERATION {i} -> {opsPerMSec} operations/millisecond");
                if (opsPerMSec > bestPerform) bestPerform = opsPerMSec;
                CleanGarbageCollector();
            }

            return bestPerform;
        }
        
        private static long Perform32Times(Action handler, long millis)
        {
            const int callCounter = 32;
            
            long current;
            long operations = 0;
            
            long start = Environment.TickCount;
            long end = start + millis;

            do
            {
                handler(); handler(); handler(); handler(); handler(); handler(); handler(); handler();
                handler(); handler(); handler(); handler(); handler(); handler(); handler(); handler();
                handler(); handler(); handler(); handler(); handler(); handler(); handler(); handler();
                handler(); handler(); handler(); handler(); handler(); handler(); handler(); handler();
                current = Environment.TickCount;
                operations += callCounter;

            } while (current < end);
            
            long duration = current - start;
            return operations / duration;
        }
    }
}