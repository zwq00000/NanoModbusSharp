namespace Nano.Modbus.Core {
    /// <summary>
    ///     Modbus 功能码
    ///     Created by zwq00000 on 2014/7/9.
    /// </summary>
    public enum FunctionCodes : byte {
        /// <summary>
        ///     读取线圈状态   取得一组逻辑线圈的当前状态（ON/OFF)
        /// </summary>
        READ_COILS = 1,

        /// <summary>
        ///     读取输入状态  取得一组开关输入的当前状态（ON/OFF)
        /// </summary>
        READ_DISCRETE_INPUTS = 2,

        /// <summary>
        ///     读取保持寄存器  在一个或多个保持寄存器中取得当前的二进制值
        /// </summary>
        READ_HOLDING_REGISTERS = 3,

        /// <summary>
        ///     读取输入寄存器 在一个或多个输入寄存器中取得当前的二进制值
        /// </summary>
        READ_INPUT_REGISTERS = 4,

        /// <summary>
        ///     强置单线圈 强置一个逻辑线圈的通断状态
        /// </summary>
        WRITE_COIL = 5,

        /// <summary>
        ///     预置单寄存器  把具体二进值装入一个保持寄存器
        /// </summary>
        WRITE_REGISTER = 6,

        /// <summary>
        ///     读取异常状态 取得8个内部线圈的通断状态，这8个线圈的地址由控制器决定
        /// </summary>
        READ_EXCEPTION_STATUS = 7,

        /// <summary>
        ///     强置多线圈 强置一串连续逻辑线圈的通断
        /// </summary>
        WRITE_COILS = 15,

        /// <summary>
        ///     写入多个保持寄存器
        /// </summary>
        WRITE_REGISTERS = 16,
        REPORT_SLAVE_ID = 17,
        WRITE_MASK_REGISTER = 22
    }
}