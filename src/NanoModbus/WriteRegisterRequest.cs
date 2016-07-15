using Nano.Modbus.Core;

namespace Nano.Modbus {
    /// <summary>
    ///     写入 单一保持寄存器
    ///     Created by zwq00000 on 2014/7/10.
    /// </summary>
    public class WriteRegisterRequest : ModbusFrame {
        /// <summary>
        ///     @param slaveId
        ///     @param regAddress
        ///     @param regValue
        /// </summary>
        public WriteRegisterRequest(byte slaveId, short regAddress, short regValue)
            : base(slaveId, FunctionCodes.WRITE_REGISTER, 4) {
            RegAddress = regAddress;
            RegValue = regValue;
            AppendValue(regAddress);
            AppendValue(regValue);
        }

        public short RegAddress { get; }

        public short RegValue { get; }

        /// <summary>
        ///     获取该请求的 功能码
        ///     {@link FunctionCode}
        ///     @return
        /// </summary>
        public override FunctionCodes FunctionCode {
            get { return FunctionCodes.WRITE_REGISTER; }
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