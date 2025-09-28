using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataFormatConverter.Domain.Entities
{
    /// <summary>
    /// Represents the input request for the Data Format Converter API.
    /// Contains the input format, output format, and the data to be converted.
    /// </summary>
    public record ConversionRequest(string inputFormat, string outputFormat, string data);
}
