using Chevron9.Bootstrap.Data;
using Chevron9.Bootstrap.Data.Configs;
using DryIoc;

namespace Chevron9.Bootstrap;

public class Chevron9Bootstrap : IDisposable
{
    private readonly Container _container;

    private readonly Chevron9Config _config;
    public Chevron9Bootstrap(Chevron9Config config)
    {
        _container = new Container();
        _config = config;
    }

    public void Dispose()
    {
        _container.Dispose();
        GC.SuppressFinalize(this);
    }


}
