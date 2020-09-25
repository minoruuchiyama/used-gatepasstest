using System;
using System.Runtime.Serialization;

namespace ReadTags.Exceptions {
    /// <summary>
    /// Http 400 Bad request 用例外
    /// </summary>
    [Serializable]
    public class Http400BadRequestException : Exception{
        public Http400BadRequestException()
            : base() {
        }

        public Http400BadRequestException(string message)
            : base(message) {
        }

        public Http400BadRequestException(string message, Exception innerException)
            : base(message, innerException) {
        }

        protected Http400BadRequestException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }
    }
}
