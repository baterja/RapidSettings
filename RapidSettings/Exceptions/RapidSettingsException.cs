using System;
using System.Runtime.Serialization;

namespace RapidSettings.Core
{
    /// <summary>
    /// Custom exception that is thrown by classes from this assembly when something wrong happens.
    /// </summary>
    [Serializable]
    public class RapidSettingsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RapidSettingsException"/> class.
        /// </summary>
        public RapidSettingsException()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RapidSettingsException"/> class with exception description.
        /// </summary>
        /// <param name="message">Exception description.</param>
        public RapidSettingsException(string message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RapidSettingsException"/> class with exception description and inner exception.
        /// </summary>
        /// <param name="message">Exception description.</param>
        /// <param name="inner">Inner exception.</param>
        public RapidSettingsException(string message, Exception inner)
            : base(message, inner)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RapidSettingsException"/> class with serialization info and streaming context. Used when exception is deserialized.
        /// </summary>
        /// <param name="info">Data for deserialization.</param>
        /// <param name="context">Contains source and target of streaming and may contains additional info for deserialization.</param>
        protected RapidSettingsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
