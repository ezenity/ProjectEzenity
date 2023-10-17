using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezenity.Core.Interfaces
{
    /// <summary>
    /// Contains settings for the applications connection strings
    /// </summary>
    public interface IConnectionStringSettings
    {
        /// <summary>
        /// Gets the SQL Connection details.
        /// </summary>
        string WebApiDatabase { get; }
    }
}
