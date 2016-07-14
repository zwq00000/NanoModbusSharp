using Nano.Modbus.Core;

namespace Nano.Modbus {
    /// <summary>
    ///     读线圈器请求
    ///     <see cref="FunctionCodes.READ_COILS" />
    /// </summary>
    public class ReadCoilsRequest : ModbusFrame {
        public ReadCoilsRequest(byte alaveId, short startCoilNum, short coilsCount) :
            this(alaveId, new CoilStatus(startCoilNum, coilsCount)) {
        }

        public ReadCoilsRequest(int slaveId, CoilStatus coilStatus) : base((byte) slaveId, FunctionCodes.READ_COILS, 4) {
            Coils = coilStatus;
            AppendValue(coilStatus.StartNum);
            AppendValue(coilStatus.Count);
        }

        /// <summary>
        /// 线圈状态
        /// </summary>
        public CoilStatus Coils { get; }

        /// <summary>
        ///     获取该请求的 功能码
        ///     {@link FunctionCode}
        ///     @return
        /// </summary>
        public override FunctionCodes FunctionCode {
            get { return FunctionCodes.READ_COILS; }
        }


        /// <summary>
        ///     读取响应数据流
        ///     @param responseBuffer
        /// </summary>
        protected override bool ReadResponse(byte[] responseBuffer, int length) {
            var slaveId = responseBuffer[0];
            if (slaveId != SlaveId) {
                // 从站ID 不符
                return false;
            }
            var funcCode = (FunctionCodes) responseBuffer[1];
            if (funcCode != FunctionCode) {
                return false;
            }
            Coils.ReadResponse(responseBuffer, length);
            return true;
        }

        /// <summary>
        ///     PDU： 协议数据单元 长度 包括 功能码 和 数据 不包括 地址域 和 CRC 校验
        /// </summary>
        protected override int GetPduLen() {
            return (Coils.Count + 7)/8 + 2;
        }
    }
}