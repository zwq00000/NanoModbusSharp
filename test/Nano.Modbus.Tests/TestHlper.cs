using System;
using System.IO;
using System.IO.Ports;

namespace Nano.Modbus.Tests {
    public static class TestHlper {
        public static SerialPort createSerialPort() {
            throw new NotImplementedException();
        }

        public static Stream CreateStream() {
            return new MemoryStream();
        }
    }
}