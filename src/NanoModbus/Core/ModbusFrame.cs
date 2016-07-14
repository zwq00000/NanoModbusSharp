using System;
using System.IO;

namespace Nano.Modbus.Core {
    /// <summary>
    ///     Modbus 数据帧
    /// </summary>
    public abstract class ModbusFrame {
        /// <summary>
        ///     响应缓冲
        /// </summary>
        private static readonly byte[] RESPONSE_BUFFER = new byte[128];

        private static readonly byte[] CRC_BUFFER = new byte[2];
        private readonly object _syncRoot = new object();

        /// <summary>
        ///     帧长度
        /// </summary>
        private int _length;

        /// <summary>
        ///     数据帧
        /// </summary>
        private byte[] _mFrame;

        /// <summary>
        ///     Modbus 数据帧 构造方法
        /// </summary>
        /// <param name="slaveId">从站Id</param>
        /// <param name="funcCode">功能码</param>
        protected ModbusFrame(byte slaveId, FunctionCodes funcCode) {
            _mFrame = new byte[8];
            _mFrame[0] = slaveId;
            _mFrame[1] = (byte) funcCode;
            _length = 2;
        }

        /// <summary>
        ///     数据帧构造方法
        /// </summary>
        /// <param name="slaveId">从站Id</param>
        /// <param name="funcCode">功能码  {@link FunctionCodes}</param>
        /// <param name="valueSize">帧中数值部分大小（不包括头部）</param>
        protected ModbusFrame(byte slaveId, FunctionCodes funcCode, int valueSize) {
            _mFrame = new byte[valueSize + 2];
            _mFrame[0] = slaveId;
            _mFrame[1] = (byte) funcCode;
            _length = 2;
        }

        /// <summary>
        ///     数据帧构造方法
        /// </summary>
        /// <param name="slaveId">从站Id</param>
        /// <param name="funcCode"> 功能码 {@link FunctionCodes}</param>
        /// <param name="values">数值部分（不包括头部）</param>
        protected ModbusFrame(byte slaveId, FunctionCodes funcCode, byte[] values) {
            _length = 2 + values.Length;
            _mFrame = new byte[_length];
            _mFrame[0] = slaveId;
            _mFrame[1] = (byte) funcCode;
            Array.Copy(values, 0, _mFrame, 2, values.Length);
        }

        /// <summary>
        ///     数据帧构造方法
        /// </summary>
        /// <param name="slaveId">从站Id</param>
        /// <param name="funcCode">功能码 {@link FunctionCodes}</param>
        /// <param name="startNum">寄存器起始位置</param>
        /// <param name="coilsCount">寄存器数量</param>
        protected ModbusFrame(byte slaveId, FunctionCodes funcCode, short startNum, short coilsCount) {
            _length = 2 + 4;
            _mFrame = new byte[_length];
            _mFrame[0] = slaveId;
            _mFrame[1] = (byte) funcCode;
            _mFrame.SetBytes(2, startNum);
            _mFrame.SetBytes(4, coilsCount);
        }

        /// <summary>
        ///     数据帧构造方法
        /// </summary>
        /// <param name="funcCode">功能码 {@link FunctionCodes}</param>
        /// <param name="holder">寄存器数据保持器</param>
        protected ModbusFrame(FunctionCodes funcCode, IHolder<short> holder)
            : this(holder.SlaveId, funcCode, holder.StartNum, holder.Count) {
        }

        /// <summary>
        ///     从站设备 Id
        /// </summary>
        protected byte SlaveId {
            get {
                if (_mFrame.Length > 0) {
                    return _mFrame[0];
                }
                return 0;
            }
            set {
                if (_mFrame.Length > 0) {
                    _mFrame[0] = value;
                }
            }
        }


        /// <summary>
        ///     获取该请求的 功能码 {@link FunctionCodes}
        /// </summary>
        public virtual FunctionCodes FunctionCode {
            get { return (FunctionCodes) _mFrame[1]; }
        }

        /// <summary>
        ///     写入数据之前,数据预处理
        /// </summary>
        protected virtual void OnBeginWriteFrame() {
        }

        /// <summary>
        ///     发生在 读取数据之前的事件
        /// </summary>
        protected virtual void OnBeginReadFrame() {
        }


        /// <summary>
        ///     写入 数据帧 到输出流
        /// </summary>
        /// <param name="outputStream"></param>
        public void WriteFrame(Stream outputStream) {
            if (outputStream == null) {
                throw new ArgumentNullException(nameof(outputStream));
            }
            OnBeginWriteFrame();
            outputStream.Write(_mFrame, 0, _length);
            var crc = GetCRC();
            outputStream.Write(crc, 0, crc.Length);
            outputStream.Flush();
        }

        /// <summary>
        ///     读取响应数据流
        /// </summary>
        public bool ReadResponse(Stream inputStream) {
            if (inputStream == null) {
                throw new ArgumentNullException(nameof(inputStream), "input stream is not been Null");
            }
            OnBeginReadFrame();
            var aduLen = GetPduLen() + 1;
            var len = ReadStream(inputStream, RESPONSE_BUFFER, 0, aduLen);
            if (len < aduLen) {
                //读取失败
                return false;
            }
            if (RESPONSE_BUFFER[0] != SlaveId) {
                //slaveId 不正确
                return false;
            }
            if (RESPONSE_BUFFER[1] != (int) FunctionCode) {
                //功能码不正确
                return false;
            }
            len = ReadStream(inputStream, CRC_BUFFER);
            if (len < 2) {
                //CRC16 读取错误
                return false;
            }
            if (ValidateCrc16(RESPONSE_BUFFER, aduLen, CRC_BUFFER)) {
                throw new IOException("CRC16 校验错误");
            }
            return ReadResponse(RESPONSE_BUFFER, aduLen);
        }

        /// <summary>
        ///     验证 CRC16 校验
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="aduLen"></param>
        /// <param name="crc"></param>
        /// <returns></returns>
        private static bool ValidateCrc16(byte[] frame, int aduLen, byte[] crc) {
            var result = CRC16.Calculate(frame, aduLen - 2);
            return result[0] == crc[0] && result[1] == crc[1];
        }

        /// <summary>
        ///     从输入流中循环读取数据 填充到 buffer 中
        ///     @param inputStream
        ///     @param buffer
        ///     @throws IOException
        /// </summary>
        private static int ReadStream(Stream inputStream, byte[] buffer) {
            return ReadStream(inputStream, buffer, 0, buffer.Length);
        }

        /// <summary>
        ///     从输入流中循环读取数据 填充到 buffer 中
        ///     @param inputStream
        ///     @param buffer
        ///     @throws IOException
        /// </summary>
        private static int ReadStream(Stream inputStream, byte[] buffer, int off, int length) {
            var retryCount = 0;
            var pos = off;
            while (pos < length) {
                var readCount = inputStream.Read(buffer, pos, length - pos);
                if (readCount < 0) {
                    retryCount++;
                    if (retryCount > 3) {
                        break;
                    }
                } else {
                    pos += readCount;
                }
            }
            return pos;
        }

        /// <summary>
        ///     读取响应数据
        /// </summary>
        protected abstract bool ReadResponse(byte[] responseBuffer, int length);

        /// <summary>
        ///     PDU： 协议数据单元 长度
        ///     包括 功能码 和 数据
        ///     不包括 地址域 和 CRC 校验
        ///     @return
        /// </summary>
        protected abstract int GetPduLen();


        public byte[] GetCRC() {
            return CRC16.Calculate(_mFrame, _length);
        }

        public byte[] GetValues() {
            return _mFrame;
        }

        public byte[] ToBytes() {
            var result = new byte[_mFrame.Length + 2];
            var crc = GetCRC();
            Array.Copy(_mFrame, 0, result, 0, _mFrame.Length);
            result[_mFrame.Length] = crc[0];
            result[_mFrame.Length + 1] = crc[1];
            return result;
        }

        /// <summary>
        ///     设置数据部分,起始位置为 2
        /// </summary>
        public void SetValues(byte[] values) {
            if (_mFrame.Length < values.Length + 2) {
                _mFrame = ByteUtils.Combine(_mFrame, 2, values);
            } else {
                Array.Copy(values, 0, _mFrame, 2, values.Length);
            }
            _length = values.Length + 2;
        }

        /// <summary>
        ///     增加数值部分
        /// </summary>
        protected void AppendValue(byte value) {
            if (_mFrame.Length < (_length + 1)) {
                //扩容
                _mFrame = ByteUtils.Combine(_mFrame, new byte[8]);
            }
            _mFrame[_length] = value;
            _length++;
        }

        /// <summary>
        ///     增加数值部分
        /// </summary>
        protected void AppendValue(byte[] bytes) {
            if (_mFrame.Length < (_length + bytes.Length)) {
                //扩容
                _mFrame = ByteUtils.Combine(_mFrame, bytes);
            } else {
                Array.Copy(bytes, 0, _mFrame, _length, bytes.Length);
            }
            _length += bytes.Length;
        }

        /// <summary>
        ///     增加数值部分
        /// </summary>
        protected void AppendValue(short shortValue) {
            lock (_syncRoot) {
                var bytes = ByteUtils.ToBytes(shortValue);
                AppendValue(bytes);
            }
        }
    }
}