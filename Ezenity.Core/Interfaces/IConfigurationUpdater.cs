using Microsoft.Extensions.Configuration;

namespace Ezenity.Core.Interfaces
{
    public interface IConfigurationUpdater
    {
        IConfigurationRoot UpdateConfiguration(IConfiguration configuration);
    }
}
