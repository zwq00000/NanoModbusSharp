using System;
using System.IO.Ports;
using System.Threading;
using Nano.Modbus;
using Nano.Modbus.Core;
using NUnit.Framework;

//using NUnit.Framework;

namespace NanoModbusTests {
    [TestFixture]
    public class CoilStatusTest {
        [Test]
        public void TestGetCoil() {
            testGetSetCoils(4);
            testGetSetCoils(8);
            testGetSetCoils(14);
            testGetSetCoils(254);
        }

        [Test]
        public void testGetCount() {
            var coilsCount = 12;
            var coils = new CoilStatus(coilsCount);
            Assert.AreEqual(coils.Count, coilsCount);
        }

        [Test]
        private void testGetSetCoils(int coilsCount) {
            var coils = new CoilStatus(coilsCount);
            for (var i = 0; i < coilsCount; i++) {
                Assert.AreEqual(coils.GetCoil(i), false);
                Console.WriteLine(ByteUtils.ToHexString(coils.ToBytes()));
            }
            for (var i = 0; i < coilsCount; i++) {
                coils.SetCoil(i, true);
                Assert.AreEqual(coils.GetCoil(i), true);
                Console.WriteLine(ByteUtils.ToHexString(coils.ToBytes()));
            }

            for (var i = 0; i < coilsCount; i++) {
                coils.SetCoil(i, false);
                Assert.AreEqual(coils.GetCoil(i), false);
                Console.WriteLine(ByteUtils.ToHexString(coils.ToBytes()));
            }
        }

        [Test]
        public void testReadResponse1() {
            var portName = "COM4"; // "/dev/ttyS1";

            using (var port = new SerialPort(portName, 115200, 0)) {
                port.Open();
                var coils = new CoilStatus(4);
                var frame = ModbusFactory.CreateWriteCoilsFrame(1, coils);
                var bytes = frame.ToBytes();
                port.Write(bytes, 0, bytes.Length);

                Thread.Sleep(100);
                var buf = new byte[255];
                var len = port.Read(buf, 0, buf.Length);
                Assert.AreEqual(coils.GetCoil(0), false);
            }
        }

        [Test]
        public void testSetCoil() {
        }

        [Test]
        public void TestToBytes() {
            var coils = new CoilStatus(4);
            Console.WriteLine(ByteUtils.ToHexString(coils.ToBytes()));
            Assert.AreEqual(ByteUtils.ToHexString(coils.ToBytes()), "00 00 00 04 01 00");

            coils.SetCoil(0, true);
            Console.WriteLine(ByteUtils.ToHexString(coils.ToBytes()));
            Assert.AreEqual(ByteUtils.ToHexString(coils.ToBytes()), "00 00 00 04 01 01");
        }
    }
}