using System;
using System.Runtime.Serialization;

namespace ReadTags.Exceptions {
    /// <summary>
    /// Http 404 Not found 用例外
    /// </summary>
    [Serializable]
    public class Http404NotFoundException : Exception{
        public Http404NotFoundException()
            : base() {
        }

        public Http404NotFoundException(string message)
            : base(message) {
        }

        public Http404NotFoundException(string message, Exception innerException)
            : base(message, innerException) {
        }

        protected Http404NotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }
    }
}
