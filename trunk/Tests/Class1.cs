using System;
using System.Collections.Generic;
using System.Text;
using NMock2;
using NUnit.Framework;


namespace NCoverCop.Tests
{
    public class MockingTestFixture
    {
        private Mockery mockery;
        protected virtual void SetUp()
        {
            mockery = new Mockery();
        }
    }
}
