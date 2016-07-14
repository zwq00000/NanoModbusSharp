using System;
using System.Linq;
using System.Text;
using Nano.Modbus.Core;

namespace Nano.Modbus {
    public class RegisterHolder : IWriteableHolder<short> {
        private readonly short[] _mStatus;

        public RegisterHolder(byte slaveId, int regCount) : this(slaveId, 0, (short) regCount) {
        }

        public RegisterHolder(byte slaveId, short startNum, int regCount) {
            StartNum = startNum;
            SlaveId = slaveId;
            _mStatus = new short[regCount];
        }

        /// <summary>
        ///     设置和获取 寄存器 数值
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public short this[int offset] {
            get {
                if (offset < 0 || offset >= _mStatus.Length) {
                    throw new ArgumentOutOfRangeException(nameof(offset), $"offset 超出范围 [0,{_mStatus.Length - 1}]");
                }
                return _mStatus[offset];
            }
            set {
                if (offset < 0 || offset >= _mStatus.Length) {
                    throw new ArgumentOutOfRangeException(nameof(offset), $"offset 超出范围 [0,{_mStatus.Length - 1}]");
                }
                _mStatus[offset] = value;
            }
        }

        public byte SlaveId { get; }
        public short StartNum { get; }

        public short Count {
            get { return (short) _mStatus.Length; }
        }

        public void Reset() {
            Array.Clear(_mStatus, 0, _mStatus.Length);
        }

        public byte[] ToBytes() {
            var bytes = new byte[2 + 2 + 1 + _mStatus.Length*2];
            bytes.SetBytes(0, StartNum);
            bytes.SetBytes(2, (short) _mStatus.Length);
            bytes[4] = (byte) (_mStatus.Length*2);
            for (var i = 0; i < _mStatus.Length; i++) {
                bytes.SetBytes((i*2) + 5, _mStatus[i]);
            }
            return bytes;
        }

        /// <summary>
        ///     获取 寄存器状态数据帧 占用字节数
        ///     包括 起始地址 + 数量 + 字节数 + 状态字节数组
        /// </summary>
        /// <returns></returns>
        public int Size() {
            return 2 + 2 + 1 + _mStatus.Length*2;
        }

        public override string ToString() {
            var builder = new StringBuilder("HoldingRegister[");
            var items = _mStatus.Select((index, s) => $"{StartNum + index}:{s}");
            var enumerator = items.GetEnumerator();
            if (enumerator.MoveNext()) {
                builder.Append(enumerator.Current);
                while (enumerator.MoveNext()) {
                    builder.Append(",")
                        .Append(enumerator.Current);
                }
            }

            builder.Append(']');
            return builder.ToString();
        }
    }
}