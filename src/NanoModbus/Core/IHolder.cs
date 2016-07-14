namespace Nano.Modbus.Core {
    /// <summary>
    ///     寄存器状态 保持器
    /// </summary>
    public interface IHolder<T> {
        /// <summary>
        ///     读取/设置寄存器 原始值
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        T this[int offset] { get; set; }

        /// <summary>
        ///     获取从站Id    {@value 1~255}
        /// </summary>
        /// <returns></returns>
        byte SlaveId { get; }


        /// <summary>
        ///     寄存器 起始位置
        /// </summary>
        /// <returns></returns>
        short StartNum { get; }

        /// <summary>
        ///     寄存器 数量
        /// </summary>
        short Count { get; }

        /// <summary>
        ///     读取之前 重置寄存器数值
        /// </summary>
        void Reset();
    }
}