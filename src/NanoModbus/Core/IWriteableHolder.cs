namespace Nano.Modbus.Core {
    /// <summary>
    ///     可写的状态保持器
    /// </summary>
    public interface IWriteableHolder<T> : IHolder<T> {
        /// <summary>
        ///     返回全部数据
        /// </summary>
        byte[] ToBytes();

        /// <summary>
        ///     获取 寄存器状态数据帧 占用字节数
        ///     包括 起始地址 + 数量 + 字节数 + 状态字节数组
        /// </summary>
        int Size();
    }
}