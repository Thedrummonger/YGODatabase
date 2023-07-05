using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YGODatabase
{
    internal class CodeTimer
    {
        private readonly Dictionary<int, Stopwatch> stopwatches = new();
        private Dictionary<string, List<double>> Data = new Dictionary<string, List<double>>();
        public CodeTimer StartNew(int ID)
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();
            stopwatches[ID] = stopwatch;
            return this;
        }

        public void TimeSegment(int ID, string CodeSegment)
        {
            if (!Data.ContainsKey(CodeSegment)) { Data[CodeSegment] = new List<double>(); }
            Data[CodeSegment].Add(stopwatches[ID].Elapsed.TotalMilliseconds);
            stopwatches[ID].Reset();
            stopwatches[ID].Start();
        }

        public void PrintData()
        {
            foreach(var kvp in stopwatches.Values)
            {
                kvp.Reset();
                kvp.Stop();
            }
            foreach (var i in Data)
            {
                Debug.WriteLine($"{i.Key} ran {i.Value.Count} | average {i.Value.Average()} | total time {i.Value.Sum()}");
            }
        }
    }
}
