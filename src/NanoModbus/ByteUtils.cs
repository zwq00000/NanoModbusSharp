using System;
using System.Linq;

namespace Nano.Modbus {
    public static class ByteUtils {
        private static readonly byte[] ClearBits = {
            0xFE, 0xFD, 0xFB,
            0xF7, 0xEF, 0xDF, 0xBF, 0x7F
        };

        /// <summary>
        ///     int到byte[] 高位在前
        ///     @param i
        ///     @return
        /// </summary>
        public static byte[] ToBytes(int i) {
            //由高位到低位
            return new[] {
                (byte) ((i >> 24) & 0xFF),
                (byte) ((i >> 16) & 0xFF),
                (byte) ((i >> 8) & 0xFF),
                (byte) (i & 0xFF)
            };
        }

        /// <summary>
        ///     int到byte[] 高位在前
        ///     @param s
        ///     @return
        /// </summary>
        public static byte[] ToBytes(short s) {
            return new[] {(byte) ((s & 0xFF00) >> 8), (byte) (s & 0x00FF)};
        }

        /// <summary>
        ///     byte[]转 short
        ///     @param bytes
        ///     @param positon 开始位置
        ///     @return
        /// </summary>
        public static short BytesToShort(byte[] bytes, int positon) {
            if (bytes == null) {
                throw new ArgumentNullException(nameof(bytes), "参数 bytes 不能为空");
            }
            if (positon < 0) {
                throw new ArgumentOutOfRangeException(nameof(positon), "参数 position 必须大于等于 0");
            }
            if (bytes.Length < 2) {
                throw new ArgumentOutOfRangeException(nameof(bytes), "参数 bytes 长度必须大于等于 2");
            }
            if (bytes.Length < (positon + 2)) {
                throw new ArgumentOutOfRangeException(nameof(bytes), "从开始位置到结束,长度必须大于等于 2");
            }
            return (short) ((bytes[positon] & 0xff) << 8 | ((bytes[positon + 1] & 0xff)));
        }

        /// <summary>
        ///     根据 status 条件设置 字节中指定的 位（Bit）
        ///     @param bits  需要设置的字节
        ///     @param offset    字节中待指定的位置 [0,7]
        ///     @param status    true 置 1，false 置 0
        ///     @return
        /// </summary>
        public static byte SetBit(byte bits, int offset, bool status) {
            if (status) {
                return SetBit(bits, offset);
            }
            return ClearBit(bits, offset);
        }

        /// <summary>
        ///     ///  字节中指定 Bit 位置 置 1
        ///     @param bits
        ///     @param offset
        ///     @return
        /// </summary>
        public static byte SetBit(byte bits, int offset) {
            if (offset < 0 && offset > 7) {
                throw new IndexOutOfRangeException("offset 必须在[0,8)范围");
            }
            bits |= (byte) (0x1 << offset);
            return bits;
        }

        /// <summary>
        ///     字节中指定 Bit 位置 置 0
        ///     @param bits
        ///     @param offset
        ///     @return
        /// </summary>
        public static byte ClearBit(byte bits, int offset) {
            if (offset < 0 && offset > 7) {
                throw new IndexOutOfRangeException("offset 必须在[0,8)范围");
            }
            bits &= ClearBits[offset];
            return bits;
        }

        /// <summary>
        ///     比较两个字节数组是否相等
        ///     @param src
        ///     @param target
        ///     @return
        /// </summary>
        public static bool Compare(byte[] src, byte[] target) {
            if (src == null && target == null) {
                return true;
            }
            if (src == null || target == null) {
                return false;
            }
            if (src.Length != target.Length) {
                return false;
            }
            for (var i = 0; i < target.Length; i++) {
                if (src[i] != target.Length) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        ///     字节数组 转换为 16进制 字符串
        ///     @param values    待转换的字节数组，不可为空
        ///     @return  转换后的字符串，没有分隔符
        /// </summary>
        public static string ToHexString(this byte[] values) {
            return ToHexString(values, values.Length);
        }

        /// <summary>
        ///     字节数组 转换为 16进制 字符串
        ///     @param values    待转换的字节数组，不可为空
        ///     @return  转换后的字符串，没有分隔符
        /// </summary>
        public static string ToHexString(this byte[] values, int length) {
            if (values == null) {
                throw new ArgumentNullException(nameof(values));
            }
            return string.Join(" ", values.Take(length).Select(b => b.ToString("X2")));
        }

/*        /// <summary>
        ///  16进制 字符串 转换为 字节数组
        ///  @param src 16进制字符串 如 1H 2B ...
        ///  @return
        /// </summary>
        public static byte[] HexString2Bytes(String src) {
            int len = src.Length;
            byte[] ret = new byte[len / 2 + 2];
            byte[] tmp = src.getBytes(Charset.forName("US-ASCII"));
            for (int i = 0; i < len; i += 2) {
                ret[i / 2] = uniteBytes(tmp[i], tmp[i + 1]);
            }
            return ret;
        }*/

/*        /// <summary>
        ///  16进制字符 合并为一个字节
        /// 
        ///  @param src0 16进制 高位
        ///  @param src1 16进制 低位
        /// </summary>
        private static byte uniteBytes(byte src0, byte src1) {
            byte _b0 = Byte.Parse("0x" + new String(new byte[] { src0 }));
            _b0 = (byte)(_b0 << 4);
            byte _b1 = Byte.Parse("0x" + new String(new byte[] { src1 }));
            return (byte)(_b0 ^ _b1);
        }*/


        /// <summary>
        ///     向目标数组 赋值
        ///     @param dst       目标数组
        ///     @param offset    目标数组偏移量
        ///     @param values    赋值内容
        ///     @return
        /// </summary>
        public static int SetBytes(this byte[] dst, int offset, byte[] values) {
            if (dst.Length < (offset + values.Length)) {
                throw new IndexOutOfRangeException("offset + values.Length 超出 src数组长度");
            }
            Array.Copy(values, 0, dst, offset, values.Length);
            return offset + values.Length;
        }

        public static int SetBytes(this byte[] src, int srcPos, short value) {
            return SetBytes(src, srcPos, ToBytes(value));
        }

        /// <summary>
        ///     数组合并
        /// </summary>
        /// <param name="src">合并的第一个数组</param>
        /// <param name="srcLen">第一数组长度</param>
        /// <param name="target">第二数组</param>
        /// <returns>
        ///     合并后的结果
        /// </returns>
        public static byte[] Combine(byte[] src, int srcLen, byte[] target) {
            var combined = new byte[srcLen + target.Length];
            Array.Copy(src, 0, combined, 0, srcLen);
            Array.Copy(target, 0, combined, srcLen, target.Length);
            return combined;
        }

        /// <summary>
        ///     数组合并
        /// </summary>
        /// <param name="src">合并的第一个数组</param>
        /// <param name="target">第二数组</param>
        /// <returns>合并后的结果</returns>
        public static byte[] Combine(byte[] src, byte[] target) {
            if (src == null) {
                return target;
            }
            if (target == null) {
                return src;
            }
            var combined = new byte[src.Length + target.Length];
            Array.Copy(src, 0, combined, 0, src.Length);
            Array.Copy(target, 0, combined, src.Length, target.Length);
            return combined;
        }
    }
}