using Nano.Modbus.Core;

namespace Nano.Modbus {
    /// <summary>
    ///     输入寄存器 读取请求 {@see FunctionCode.READ_INPUT_REGISTERS}
    ///     Created by zwq00000 on 2015/6/3.
    /// </summary>
    public class ReadInputRegistersRequest : ModbusFrame {
        private readonly IHolder<short> _mHolder;

        /// <summary>
        ///     @param slaveId
        /// </summary>
        public ReadInputRegistersRequest(byte slaveId, IHolder<short> holder)
            : base(slaveId, FunctionCodes.READ_INPUT_REGISTERS, holder.StartNum, holder.Count) {
            _mHolder = holder;
        }

        /// <summary>
        ///     @param holder
        /// </summary>
        public ReadInputRegistersRequest(IHolder<short> holder) : this(holder.SlaveId, holder) {
        }


        protected override void OnBeginReadFrame() {
            _mHolder.Reset();
        }

        protected override bool ReadResponse(byte[] responseBuffer, int length) {
            var regCount = responseBuffer[2]/2;
            for (var i = 0; i < regCount; i++) {
                var regValue = ByteUtils.BytesToShort(responseBuffer, (i*2 + 3));
                _mHolder[i] = regValue;
            }
            return true;
        }

        protected override int GetPduLen() {
            return 2 + _mHolder.Count*2;
        }
    }
}