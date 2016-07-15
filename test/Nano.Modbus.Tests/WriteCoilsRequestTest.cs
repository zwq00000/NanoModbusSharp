using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nano.Modbus.Core;
using Xunit;

namespace Nano.Modbus.Tests {
    public class WriteCoilsRequestTest {

        private SerialPort port;

        [Fact]
        public void FunctionCodeTest() {
            WriteCoilsRequest frame = new WriteCoilsRequest((byte)1, new CoilStatus(4));
            Console.WriteLine(frame.ToBytes().ToHexString());
            Assert.Equal(frame.ToBytes().ToHexString(), "");
        }

        //[Fact]
        public void WriteFrameTest() {
            CoilStatus coils = new CoilStatus(4);
            coils.SetCoil(1, true);
            coils.SetCoil(2, true);
            WriteCoilsRequest frame = new WriteCoilsRequest((byte)1, coils);
            Console.WriteLine(frame.ToBytes().ToHexString());

            using (var stream = TestHlper.CreateStream()) {

                for (int i = 0; i < 10; i++) {
                    coils.SetCoil(1, true);
                    coils.SetCoil(2, true);
                    frame.WriteFrame(stream);
                    frame.ReadResponse(stream);

                    Thread.Sleep(100);

                    coils.SetCoil(1, false);
                    coils.SetCoil(2, false);
                    frame.WriteFrame(stream);
                    frame.ReadResponse(stream);
                    Thread.Sleep(100);
                }
            }
        }

        [Fact]
        public void WriteCoilsResponseTest() {
            CoilStatus coils = new CoilStatus(4);
            coils.SetCoil(1, true);
            coils.SetCoil(2, true);
            WriteCoilsRequest frame = new WriteCoilsRequest((byte)1, coils);
            Console.WriteLine(frame.ToBytes().ToHexString());

            ModbusFrame frame1 = ModbusFactory.CreateWriteCoilsFrame((byte)1, coils);
            Assert.Equal(frame.ToBytes().ToHexString(), frame1.ToBytes().ToHexString());
        }

    }
}
