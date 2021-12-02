using System;
using System.Collections.Generic;
using System.Text;

namespace DUnit
{
    class TestRunException : Exception
    {
        public TestRunException()
        {
        }

        public TestRunException(string message)
            : base(message)
        {
        }

        public TestRunException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
