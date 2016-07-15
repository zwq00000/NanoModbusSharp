using Nano.Modbus.Core;

namespace Nano.Modbus {
    public class WriteCoilRequest : ModbusFrame {
        private static readonly ushort COIL_OPEN = 0xff00;
        private static readonly short COIL_CLOSE = 0x0;

        public WriteCoilRequest(byte slaveId, short startCoilNum, bool coilStatus)
            : base(slaveId, FunctionCodes.WRITE_COIL, 4) {
            AppendValue(startCoilNum);
            AppendValue(coilStatus ? (short) COIL_OPEN : COIL_CLOSE);
        }

        /// <summary>
        ///     获取该请求的 功能码 {@link FunctionCodes}
        /// </summary>
        public override FunctionCodes FunctionCode {
            get { return FunctionCodes.WRITE_COIL; }
        }


        /// <summary>
        ///     读取响应数据
        /// </summary>
        protected override bool ReadResponseInternal(byte[] responseBuffer, int length) {
            return false;
        }

        /// <summary>
        ///     PDU： 协议数据单元 长度
        ///     包括 功能码 和 数据
        ///     不包括 地址域 和 CRC 校验
        ///     @return
        /// </summary>
        protected override int GetPduLen() {
            return 4;
        }
    }
}