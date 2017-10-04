using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Security.Cryptography;

namespace ReactivePlayer.Benchmarks
{
    public sealed class Hashing_NonCrypto
    {
        private const int N = 10000;
        private readonly byte[] data;

        private readonly SHA256 sha256 = SHA256.Create();
        private readonly MD5 md5 = MD5.Create();

        public Hashing_NonCrypto()
        {
            data = new byte[N];
            new Random((int)DateTime.Now.Ticks).NextBytes(data);
        }

        [Benchmark]
        public byte[] Sha256() => sha256.ComputeHash(data);

        [Benchmark]
        public byte[] Md5() => md5.ComputeHash(data);

        //[Benchmark]
        //public byte[] DotNetGetHashCode() => byte.Parse(data.GetHashCode());
    }
}