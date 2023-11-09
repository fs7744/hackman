using BenchmarkDotNet.Attributes;
using Hackman;

[AllStatisticsColumn]
public class IdGeneratorTest
{
    private readonly IdGenerator idGenerator;

    public IdGeneratorTest()
    {
        idGenerator = new IdGenerator();
    }

    [Benchmark]
    public long IdGenerator() => idGenerator.NextId();

    [Benchmark]
    public Guid NewGuid() => Guid.NewGuid();
}