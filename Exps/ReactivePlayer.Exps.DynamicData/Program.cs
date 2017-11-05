using DynamicData;
using DynamicData.Binding;
using DynamicData.Aggregation;
using DynamicData.Operators;
using DynamicData.List;
using DynamicData.Kernel;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Tools.DynamicData
{
    class Program
    {
        const int MaxQueueItemsCount = 5;

        static void Main(string[] args)
        {
            Observable
                .Range(1, 20)
                .ToObservableChangeSet(MaxQueueItemsCount)
                .QueryWhenChanged()
                .Subscribe(numbers => Console.WriteLine(string.Join("\t", numbers)));

            Console.ReadKey();
        }
    }
}