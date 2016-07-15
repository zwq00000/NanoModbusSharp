using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nano.Modbus;
using Xunit;

namespace NanoModbusTests {
    public class RegisterHolderTest {
        [Fact]
        public void TestToBytes() {
            RegisterHolder holder = new RegisterHolder((byte)1, (short)4, 5);
            Console.WriteLine(ByteUtils.ToHexString(holder.ToBytes()));
            Assert.Equal(ByteUtils.ToHexString(holder.ToBytes()), "9C 41 00 05 0A 00 00 00 00 00 00 00 00 00 00");

            holder[0] = 2;
            holder[1] = 500;
            holder[2] = 200;
            holder[3] = 4;
            holder[4] = 5000;
            Console.WriteLine(ByteUtils.ToHexString(holder.ToBytes()));
        }

        [Fact]
        public void testSize() {
            RegisterHolder holder = new RegisterHolder((byte)1, (short)4, (short)5);
            Assert.Equal(holder.Size(), 5 + 10);
            Console.WriteLine(holder.ToBytes().ToHexString());
        }
    }
}
