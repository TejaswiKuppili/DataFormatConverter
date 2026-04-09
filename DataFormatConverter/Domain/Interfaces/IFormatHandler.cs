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
        string FormatKey { get; }
        object Deserialize(string data);
        string Serialize(object obj);
    }
}
