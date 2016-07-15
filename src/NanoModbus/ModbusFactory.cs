using System;
using Nano.Modbus.Core;

namespace Nano.Modbus
{
    public static class ModbusFactory
    {
        /// <summary>
        ///     写单个寄存器
        ///     @param slaveId      设备地址
        ///     @param startCoilNum 线圈寄存器编号
        ///     @param coilValue    线圈寄存器的值
        /// </summary>
        public static ModbusFrame CreateWriteCoilFrame(byte slaveId, short startCoilNum,
            bool coilValue) {
            //ModbusFrame frame = new WriteCoilRequest(slaveId,startCoilNum,coilValue);
            //byte[] regNumBytes = ByteUtils.ToBytes(startCoilNum);
            //frame.mFrame = new byte[]{slaveId, FunctionCodes.WRITE_COIL, regNumBytes[0], regNumBytes[1], coilValue ? (byte) 0xff : 0x00, (byte) 0};
            //return frame;
            return new WriteCoilRequest(slaveId, startCoilNum, coilValue);
        }

        /// <summary>
        ///     创建 读可读写数字量寄存器（线圈状态） 数据帧
        ///     @param alaveId       设备地址
        ///     @param startCoilNum 起始寄存器地址
        ///     @param coilsCount   读取寄存器数量
        ///     @return 用于读取寄存器状态的数据帧
        /// </summary>
        public static ModbusFrame CreateReadCoilsFrame(byte alaveId, short startCoilNum,
            short coilsCount) {
            return new ReadCoilsRequest(alaveId, startCoilNum, coilsCount);
        }

        /// <summary>
        ///     创建 读可读写数字量寄存器（线圈状态） 数据帧
        /// </summary>
        public static ModbusFrame CreateReadCoilsFrame(int device, int startRegNum, int regCount) {
            return CreateReadCoilsFrame((byte)device, (short)startRegNum, (short)regCount);
        }

        ///  <summary>
        ///      创建 写多个线圈寄存器的 Modbus 数据帧 ModbusUtils.pushShort(queue, startOffset); ModbusUtils.pushShort(queue,
        ///      numberOfBits); ModbusUtils.pushByte(queue, data.Length); 从机Id 1 BYTE 功能码 1 BYTE 0X0F 设置起始地址 2
        ///      BYTE  0X0000 TO 0XFFFF 设置长度  2 BYTE  0X0000 TO 0X7B0 字节计数 1 BYTE N 设置内容  N BYTE
        ///  </summary>
        /// <param name="slaveId">从站Id</param>
        /// <param name="coilStatus">线圈状态</param>
        public static WriteCoilsRequest CreateWriteCoilsFrame(byte slaveId, CoilStatus coilStatus) {
            if (coilStatus == null) {
                throw new ArgumentNullException(nameof(coilStatus));
            }
            return new WriteCoilsRequest(slaveId, coilStatus);
        }
    }
}
