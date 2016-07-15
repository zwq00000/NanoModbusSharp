using Nano.Modbus.Core;

namespace Nano.Modbus {
    public class WriteCoilsRequest : ModbusFrame {
        private static readonly int COILS_STATUS_POS = 7;
        private readonly CoilStatus _coils;


        /// <summary>
        ///     从机Id 1 BYTE
        ///     功能码 1 BYTE 0X0F
        ///     设置起始地址 2 BYTE  0X0000 TO 0XFFFF
        ///     设置长度  2 BYTE  0X0000 TO 0X7B0
        ///     字节计数 1 BYTE N
        ///     设置内容 N BYTE
        /// </summary>
        public WriteCoilsRequest(byte slaveId, CoilStatus coilStatus)
            : base(slaveId, FunctionCodes.WRITE_COILS, coilStatus.BytesSize) {
            _coils = coilStatus;
            SetValues(coilStatus.ToBytes());
        }

        /// <summary>
        ///     获取该请求的 功能码 {@link FunctionCodes}
        /// </summary>
        public override FunctionCodes FunctionCode {
            get { return FunctionCodes.WRITE_COILS; }
        }


        /// <summary>
        ///     写入数据之前,数据预处理
        /// </summary>
        protected override void OnBeginWriteFrame() {
            SetValues(_coils.ToBytes());
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
            return 5;
        }
    }
}