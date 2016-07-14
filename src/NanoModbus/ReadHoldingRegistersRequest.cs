using Nano.Modbus.Core;

namespace Nano.Modbus {
    /// <summary>
    ///     保持寄存器读取请求 {@see FunctionCode.READ_HOLDING_REGISTERS}
    ///     Created by zwq00000 on 2014/7/10.
    /// </summary>
    public class ReadHoldingRegistersRequest : ModbusFrame {
        private readonly RegisterHolder _mHolder;

        /**
         * @param holder
         */

        public ReadHoldingRegistersRequest(RegisterHolder holder)
            : base(holder.SlaveId, FunctionCodes.READ_HOLDING_REGISTERS, holder.StartNum,
                holder.Count) {
            _mHolder = holder;
        }

        /// <summary>
        ///     获取该请求的 功能码 {@link com.redriver.modbus.FunctionCode}
        /// </summary>
        public override FunctionCodes FunctionCode {
            get { return FunctionCodes.READ_HOLDING_REGISTERS; }
        }

        protected override void OnBeginReadFrame() {
            _mHolder.Reset();
        }

        protected override bool ReadResponse(byte[] responseBuffer, int length) {
            if (responseBuffer[1] == (int) FunctionCode) {
                int byteCount = responseBuffer[2];
                int regCount = _mHolder.Count;
                if (byteCount == regCount*2) {
                    for (var i = 0; i < regCount; i++) {
                        var regValue = ByteUtils.BytesToShort(responseBuffer, 3 + (i*2));
                        _mHolder[i] = regValue;
                    }
                }
            }
            return true;
        }

        protected override int GetPduLen() {
            return 2 + _mHolder.Count*2;
        }
    }
}