using Microsoft.Extensions.DependencyInjection;

namespace EC.Norma
{
    public interface INormaBuilder
    {
        IServiceCollection Services { get; }
    }
}
