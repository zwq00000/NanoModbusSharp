using Nano.Modbus.Core;

namespace Nano.Modbus {
    /// <summary>
    ///     写入多个 保持寄存器
    ///     Created by zwq00000 on 2014/7/10.
    /// </summary>
    public class WriteRegistersRequest : ModbusFrame {
        private readonly RegisterHolder _mHolder;

        /// <summary>
        ///     @param slaveId
        /// </summary>
        public WriteRegistersRequest(byte slaveId, RegisterHolder holder)
            : base(slaveId, FunctionCodes.WRITE_REGISTERS, holder.Size()) {
            SetValues(holder.ToBytes());
            _mHolder = holder;
        }

        /// <summary>
        ///     获取该请求的 功能码
        ///     {@link FunctionCode}
        ///     @return
        /// </summary>
        public override FunctionCodes FunctionCode {
            get { return FunctionCodes.WRITE_REGISTERS; }
        }

        protected override void OnBeginWriteFrame() {
            base.OnBeginWriteFrame();
            SetValues(_mHolder.ToBytes());
        }

        protected override void OnBeginReadFrame() {
            base.OnBeginReadFrame();
            _mHolder.Reset();
        }

        protected override bool ReadResponse(byte[] responseBuffer, int length) {
            if (responseBuffer[0] == (int) (FunctionCodes.WRITE_REGISTERS + 0x80)) {
                return false;
                // throw new IllegalArgumentException("Modbus 错误 WRITE_REGISTERS Error Code:"+Byte.toString(responseBuffer[1]));
            }
            return true;
        }

        /// <summary>
        ///     PDU： 协议数据单元 长度
        ///     包括 功能码 和 数据
        ///     不包括 地址域 和 CRC 校验
        ///     @return
        /// </summary>
        protected override int GetPduLen() {
            return _mHolder.Count*2 + 2;
        }
    }
}