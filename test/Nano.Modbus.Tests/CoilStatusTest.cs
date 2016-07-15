using System;
using System.IO.Ports;
using System.Threading;
using Xunit;

//using NUnit.Framework;

namespace Nano.Modbus.Tests {
    public class CoilStatusTest {
        [Fact]
        public void TestGetCoil() {
            testGetSetCoils(4);
            testGetSetCoils(8);
            testGetSetCoils(14);
            testGetSetCoils(254);
        }

        [Fact]
        public void TestGetCount() {
            short coilsCount = 12;
            var coils = new CoilStatus(coilsCount);
            Assert.Equal(coils.Count, coilsCount);
        }

        [Fact]
        private void testGetSetCoils(short coilsCount) {
            var coils = new CoilStatus(coilsCount);
            for (var i = 0; i < coilsCount; i++) {
                Assert.Equal(coils.GetCoil(i), false);
                Console.WriteLine(coils.ToBytes().ToHexString());
            }
            for (var i = 0; i < coilsCount; i++) {
                coils.SetCoil(i, true);
                Assert.Equal(coils.GetCoil(i), true);
                Console.WriteLine(coils.ToBytes().ToHexString());
            }

            for (var i = 0; i < coilsCount; i++) {
                coils.SetCoil(i, false);
                Assert.Equal(coils.GetCoil(i), false);
                Console.WriteLine(coils.ToBytes().ToHexString());
            }
        }

        [Fact]
        public void TestReadResponse1() {
            var portName = "COM4"; // "/dev/ttyS1";

            using (var port = new SerialPort(portName, 115200, 0) {ReadTimeout = 1000,WriteTimeout = 1000}) {
                port.Open();
                var coils = new CoilStatus(4);
                var frame = ModbusFactory.CreateWriteCoilsFrame(1, coils);
                var bytes = frame.ToBytes();
                port.Write(bytes, 0, bytes.Length);

                Thread.Sleep(100);
                var buf = new byte[255];
                var len = port.Read(buf, 0, buf.Length);
                Assert.Equal(coils.GetCoil(0), false);
            }
        }

        [Fact]
        public void testSetCoil() {
        }

        [Fact]
        public void TestToBytes() {
            var coils = new CoilStatus(4);
            Console.WriteLine(coils.ToBytes().ToHexString());
            Assert.Equal(coils.ToBytes().ToHexString(), "00 00 00 04 01 00");

            coils.SetCoil(0, true);
            Console.WriteLine(coils.ToBytes().ToHexString());
            Assert.Equal(coils.ToBytes().ToHexString(), "00 00 00 04 01 01");
        }
    }
}