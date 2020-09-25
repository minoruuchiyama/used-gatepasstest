using System;
using System.Runtime.Serialization;

namespace ReadTags.Exceptions {
    /// <summary>
    /// Http 500 Internal server error 用例外
    /// </summary>
    [Serializable]
    public class Http500InternalServerErrorException : Exception{
        public Http500InternalServerErrorException()
            : base() {
        }

        public Http500InternalServerErrorException(string message)
            : base(message) {
        }

        public Http500InternalServerErrorException(string message, Exception innerException)
            : base(message, innerException) {
        }

        protected Http500InternalServerErrorException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }
    }
}
