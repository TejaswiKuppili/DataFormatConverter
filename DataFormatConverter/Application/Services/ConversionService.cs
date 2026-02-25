using DataFormatConverter.Application.Repositories;
using DataFormatConverter.Helper;

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
            // Normalize the input data
            var rawData = DataNormalizer.ExtractDataAsString(inputData);

            // Get the appropriate handlers
            var inputHandler = _repository.GetHandler(inputFormat);
            var outputHandler = _repository.GetHandler(outputFormat);

            // Deserialize input into intermediate object (usually dictionary)
            var obj = inputHandler.Deserialize(rawData);

            // Serialize object into the desired output format and return
            return outputHandler.Serialize(obj);
        }
    }
}
