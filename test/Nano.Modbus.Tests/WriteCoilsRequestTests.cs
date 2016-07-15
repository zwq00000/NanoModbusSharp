using System;
using Xunit;

namespace Nano.Modbus.Tests {
    public class WriteCoilsRequestTests {

        [Fact]
        public void TestConstruct() {
            var request = new WriteCoilsRequest(1, new CoilStatus(4));
            Console.WriteLine(request.ToBytes().ToHexString());
        }
    }
}