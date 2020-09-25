using System;
using System.Runtime.Serialization;

namespace ReadTags.Exceptions {
    /// <summary>
    /// Http 403 Forbidden 用例外
    /// </summary>
    [Serializable]
    public class Http403ForbiddenException : Exception{
        public Http403ForbiddenException()
            : base() {
        }

        public Http403ForbiddenException(string message)
            : base(message) {
        }

        public Http403ForbiddenException(string message, Exception innerException)
            : base(message, innerException) {
        }

        protected Http403ForbiddenException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }
    }
}
