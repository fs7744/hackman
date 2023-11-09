using BenchmarkDotNet.Attributes;
using Hackman;

[AllStatisticsColumn]
public class IdGeneratorTest
{
    private readonly IdGenerator idGenerator;
    private readonly UIdGenerator uIdGenerator;

    public IdGeneratorTest()
    {
        idGenerator = new IdGenerator();
        uIdGenerator = new UIdGenerator();
    }

    [Benchmark]
    public long IdGenerator() => idGenerator.NextId();
    [Benchmark]
    public long IdGeneratorTestNextId() => idGenerator.TestNextId();
    [Benchmark]
    public ulong UIdGenerator() => uIdGenerator.NextId();
    [Benchmark]
    public ulong UIdGeneratorTestNextId() => uIdGenerator.TestNextId();

    [Benchmark]
    public string NewGuid() => Guid.NewGuid().ToString();
}