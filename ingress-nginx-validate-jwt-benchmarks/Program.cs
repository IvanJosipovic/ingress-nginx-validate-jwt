﻿using BenchmarkDotNet.Running;

namespace ingress_nginx_validate_jwt_benchmarks;

class Program
{
    static void Main(string[] args)
    {
        BenchmarkRunner.Run<Benchmark>();
    }
}