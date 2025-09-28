using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataFormatConverter.Domain.Interfaces
{
    /// <summary>
    /// Defines the contract for a data format handler.
    /// Each handler should be able to deserialize data into an object and serialize an object back into its format.
    /// </summary>
    public interface IFormatHandler
    {
        /// <summary>
        /// Key that identifies the format (e.g., "json", "xml", "csv").
        /// </summary>
        string FormatKey { get; }

        /// <summary>
        /// Deserialize input data from the specific format into an object.
        /// </summary>
        object Deserialize(string data);

        /// <summary>
        /// Serialize an object into the specific format.
        /// </summary>
        string Serialize(object obj);
    }

}
