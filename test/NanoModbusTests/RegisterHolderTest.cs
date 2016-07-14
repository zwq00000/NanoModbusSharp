using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nano.Modbus;
using NUnit.Framework;

namespace NanoModbusTests {
    [TestFixture]
    public class RegisterHolderTest {


        [Test]
        public void testToBytes() {
            RegisterHolder holder = new RegisterHolder((byte)1, (short)4, 5);
            Console.WriteLine(ByteUtils.ToHexString(holder.ToBytes()));
            Assert.AreEqual(ByteUtils.ToHexString(holder.ToBytes()), "9C4100050A00000000000000000000");

            holder[0] = 2;
            holder[1] = 500;
            holder[2] = 200;
            holder[3] = 4;
            holder[4] = 5000;
            Console.WriteLine(ByteUtils.ToHexString(holder.ToBytes()));
        }

        [Test]
        public void testSize() {
            RegisterHolder holder = new RegisterHolder((byte)1, (short)4, (short)5);
            Assert.AreEqual(holder.Size(), 5 + 10);
        }
    }
}
