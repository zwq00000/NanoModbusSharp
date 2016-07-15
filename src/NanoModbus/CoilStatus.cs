using System;
using System.Collections;
using System.Diagnostics;
using Nano.Modbus.Core;

namespace Nano.Modbus {
    /// <summary>
    ///     线圈状态
    /// </summary>
    public class CoilStatus {
        private const int SLAVE_ID_POS = 0;
        private const int FUNC_CODE_POS = 1;
        private const short MAX_COILS_COUNT = 0X7D0;
        private readonly byte[] _mStatus;
        
        public CoilStatus(short coilsCount) : this(0, coilsCount) {
        }

        /// <summary>
        ///     线圈寄存器状态
        ///     @param startNum   寄存器开始编号
        ///     @param coilsCount 寄存器数量
        /// </summary>
        public CoilStatus(short startNum, short coilsCount) {
            Debug.Assert(startNum >= 0);
            Debug.Assert((coilsCount + startNum) < MAX_COILS_COUNT);
            Count = coilsCount;
            StartNum = startNum;
            var statusSize = (coilsCount + 7)/8;
            _mStatus = new byte[statusSize];
            SetAll(false);
        }

        /// <summary>
        ///     获取线圈寄存器数量
        /// </summary>
        public short Count { get; }

        /// <summary>
        ///     寄存器组 起始编号
        /// </summary>
        public short StartNum { get; }

        /// <summary>
        ///     获取 寄存器状态数据帧 占用字节数
        ///     包括 起始地址 + 数量 + 字节数 + 状态字节数组
        ///     @return
        /// </summary>
        public int BytesSize {
            get { return 2 + 2 + 1 + _mStatus.Length; }
        }

        /// <summary>
        ///     Used internally for setting the value of the coil.
        ///     @param offset 偏移量
        ///     @param status 寄存器状态
        /// </summary>
        public void SetCoil(int offset, bool status) {
            if (offset < 0 || offset >= Count) {
                throw new ArgumentOutOfRangeException(nameof(offset), "offset 超出寄存器范围");
            }
            var byteIndex = offset/8;
            var offsetInByte = offset%8;
            _mStatus[byteIndex] = ByteUtils.SetBit(_mStatus[byteIndex], offsetInByte, status);
        }

        /// <summary>
        ///     关闭指定寄存器
        ///     @param offset
        /// </summary>
        public void Close(int offset) {
            SetCoil(offset, false);
        }

        /// <summary>
        ///     关闭指定寄存器
        ///     @param offset
        /// </summary>
        public void Open(int offset) {
            SetCoil(offset, true);
        }

        public void SetAll(bool status) {
            for (var i = 0; i < Count; i++) {
                SetCoil(i, status);
            }
        }

        /// <summary>
        ///     Returns the current value of the coil for the given offset.
        ///     @param offset
        ///     @return the value of the coil
        /// </summary>
        public bool GetCoil(int offset) {
            if (offset < 0 || offset >= Count) {
                throw new IndexOutOfRangeException("offset 超出寄存器范围");
            }
            var byteIndex = offset/8;
            var group = _mStatus[byteIndex];
            var offsetInByte = offset%8;
            var mask = (byte) (0x1 << offsetInByte);
            return (group &= mask) == mask;
        }

        public byte[] ToBytes() {
            var result = new byte[2 + 2 + 1 + _mStatus.Length];
            result.SetBytes(0, StartNum);
            result.SetBytes(2, Count);
            result[4] = (byte) _mStatus.Length;
            result.SetBytes(5, _mStatus);
            return result;
        }

        public void ReadResponse(byte[] frameBuffer, int length) {
            if (frameBuffer == null) {
                throw new ArgumentNullException(nameof(frameBuffer));
            }
            var funcCode = (FunctionCodes) frameBuffer[FUNC_CODE_POS];
            switch (funcCode) {
                case FunctionCodes.READ_COILS:
                    ReadReadCoilsResponse(frameBuffer, length);
                    break;
                case FunctionCodes.WRITE_COIL:
                    ReadWriteCoilResponse(frameBuffer, length);
                    break;
                case FunctionCodes.WRITE_COILS:
                    ReadWriteCoilsResponse(frameBuffer, length);
                    break;
            }
        }

        /// <summary>
        ///     {@link FunctionCode#WRITE_COILS}
        ///     功能码 1 BYTE 0X0F
        ///     设置起始地址 2 BYTE 0X0000 TO 0XFFFF
        ///     设置长度  2 BYTE  0X0000 TO 0X7B0
        ///     @param frameBuffer
        ///     @param length
        /// </summary>
        private void ReadWriteCoilsResponse(byte[] frameBuffer, int length) {
            var coilStartNum = ByteUtils.BytesToShort(frameBuffer, 2);
            var byteCount = ByteUtils.BytesToShort(frameBuffer, 4);
            if (byteCount != Count) {
                //todo:容量不符
                throw new ArgumentException(nameof(frameBuffer),
                    "状态容量不符 bytes=" + byteCount + "\tstatus.Length:" + _mStatus.Length);
            }
        }

        /// <summary>
        ///     读取 01 读取线圈状态 响应
        ///     功能码 1  BYTE 0X01
        ///     字节计数 1  BYTE  N
        ///     线圈状态  n  BYTE  n  =N or N+1
        ///     @param frameBuffer
        ///     @param length
        /// </summary>
        private void ReadReadCoilsResponse(byte[] frameBuffer, int length) {
            var byteCount = frameBuffer[2];
            if (byteCount != _mStatus.Length) {
                //todo:容量不符
                throw new ArgumentException(nameof(frameBuffer),
                    "状态容量不符 bytes=" + byteCount + "\tstatus.Length:" + _mStatus.Length);
            }
            for (var i = 0; i < byteCount; i++) {
                _mStatus[i] = frameBuffer[i + 3];
            }
        }

        /// <summary>
        ///     {@See ModbusFrame.FunctionCode.WRITE_COIL}
        ///     功能码 1 BYTE 0X05
        ///     设置地址 2 BYTE 0X0000 TO 0XFFFF
        ///     设置内容  2 BYTE  0x0000 OR 0XFF00
        ///     @param frameBuffer
        ///     @param length
        /// </summary>
        private void ReadWriteCoilResponse(byte[] frameBuffer, int length) {
            var coilNum = ByteUtils.BytesToShort(frameBuffer, 2);
            SetCoil(coilNum, frameBuffer[4] == 0xff);
        }
    }
}