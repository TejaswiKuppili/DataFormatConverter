using DataFormatConverter.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataFormatConverter.Application.Repositories
{
    /// <summary>
    /// Repository that maintains a mapping of supported format handlers.
    /// Provides a method to retrieve a handler by format key.
    /// </summary>
    public class FormatHandlerRepository
    {
        private readonly Dictionary<string, IFormatHandler> _handlers;

        public FormatHandlerRepository(IEnumerable<IFormatHandler> handlers)
        {
            _handlers = handlers.ToDictionary(h => h.FormatKey.ToLowerInvariant());
        }

        /// <summary>
        /// Retrieves a format handler by format key.
        /// Throws an exception if the format is unsupported.
        /// </summary>
        public IFormatHandler GetHandler(string format)
        {
            if (string.IsNullOrWhiteSpace(format))
                throw new ArgumentNullException(nameof(format), "Format cannot be null or empty.");

            var key = format.ToLowerInvariant();

            if (!_handlers.ContainsKey(key))
                throw new ArgumentException($"Format '{format}' not supported.");

            return _handlers[key];
        }
    }
}
