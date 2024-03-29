namespace Distributed.Collections.Tests;

public class InMemoryStringDistributedHashSetTests :
    StringDistributedHashSetTests<InMemoryDistributedHashSet<string>>
{
    protected override Task<InMemoryDistributedHashSet<string>> CreateHashSet() =>
        new InMemoryDistributedHashSet<string>().ToTask();
}

public class InMemoryIntDistributedHashSetTests :
    IntDistributedHashSetTests<InMemoryDistributedHashSet<int>>
{
    protected override Task<InMemoryDistributedHashSet<int>> CreateHashSet() =>
        new InMemoryDistributedHashSet<int>().ToTask();
}