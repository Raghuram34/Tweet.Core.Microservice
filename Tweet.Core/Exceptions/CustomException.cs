using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tweet.Core.Exceptions
{
    public class CustomException : Exception
    {
        public int StatusCode { get; set; } = 400;
        public CustomException()
        {
        }

        public CustomException(string message)
            : base(message)
        {
        }

        public CustomException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public CustomException(string message, int statusCode)
            : base(message)
        {
            this.StatusCode = statusCode;
        }
    }
}
