using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Benchmarks
{
    public sealed class Id3Tags
    {
        private readonly string[] filePaths;

        public Id3Tags()
        {
            this.filePaths =

            //System.IO.Directory.GetFiles(@"..\..\..\..\library\")

            new[] { @"..\..\..\..\library\London Grammar - Strong.mp3" }

            .OrderBy(s => s)
            .ToArray();
        }

        [Benchmark(Description = "TagLib# v2.1.0")]
        public IEnumerable<TagLib.Tag> TagLibSharp_v2_1_0() => filePaths.Select(path => TagLib.File.Create(path).Tag).ToArray();

        [Benchmark(Description = "CSCore v1.2.1.1 ID3v1")]
        public IEnumerable<CSCore.Tags.ID3.ID3v1> CSCore_v1_2_1_1_ID3v1() => filePaths.Select(path => CSCore.Tags.ID3.ID3v1.FromFile(path)).ToArray();

        [Benchmark(Description = "CSCore v1.2.1.1 ID3v2")]
        public IEnumerable<CSCore.Tags.ID3.ID3v2> CSCore_v1_2_1_1_ID3v2() => filePaths.Select(path => CSCore.Tags.ID3.ID3v2.FromFile(path)).ToArray();
    }
}