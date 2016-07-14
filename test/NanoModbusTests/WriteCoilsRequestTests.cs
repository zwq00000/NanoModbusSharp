using System;
using Nano.Modbus;
using NUnit.Framework;

namespace NanoModbusTests {
    [TestFixture]
    public class WriteCoilsRequestTests {

        [Test]
        public void TestConstruct() {
            var request = new WriteCoilsRequest(1, new CoilStatus(4));
            Console.WriteLine(ByteUtils.ToHexString(request.ToBytes()));
        }
    }
}