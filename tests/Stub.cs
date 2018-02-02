using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Tests
{
    public class Stub<T> : Mock<T> where T : class
    {
        public Stub() : base()
        {
        }
    }
}
