using DataFormatConverter.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataFormatConverter.Application.Services
{
    /// <summary>
    /// Service that orchestrates data conversion between formats using format handlers.
    /// </summary>
    public class ConversionService
    {
        private readonly FormatHandlerRepository _repository;

        public ConversionService(FormatHandlerRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Converts data from input format to output format.
        /// </summary>
        /// <param name="data">The data to convert</param>
        /// <param name="inputFormat">Source format</param>
        /// <param name="outputFormat">Target format</param>
        /// <returns>Converted data as string</returns>
        public string Convert(string inputData, string inputFormat, string outputFormat)
        {
            // Get the appropriate handlers
            var inputHandler = _repository.GetHandler(inputFormat);
            var outputHandler = _repository.GetHandler(outputFormat);

            // Deserialize input into intermediate object (usually dictionary)
            var obj = inputHandler.Deserialize(inputData);

            // Serialize object into the desired output format and return
            return outputHandler.Serialize(obj);
        }
    }
}
