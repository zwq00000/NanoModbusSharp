using System;
using System.Collections.Generic;
using System.Linq;

namespace Nano.Modbus.Core{
        /// <summary>
        /// Class ModbusException.
        /// </summary>
        public sealed class ModbusException : Exception {
            /// <summary>
            /// The s_ modbus exception collection
            /// </summary>
            private static List<ModbusException> _modbusExceptionCollection;

            /// <summary>
            /// Gets or sets the code.
            /// </summary>
            /// <value>The code.</value>
            public byte? Code { get; set; }

            /// <summary>
            /// Gets or sets the description.
            /// </summary>
            /// <value>The description.</value>
            public string Description { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="ModbusException" /> class.
            /// </summary>
            public ModbusException() {
            }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModbusException" /> class.
        /// </summary>
        public ModbusException(byte code) {
            this.Code = code;
            Description = GetModbusException(code).Description;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModbusException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ModbusException(string message)
                : base(message) {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ModbusException" /> class.
            /// </summary>
            /// <param name="message">The message.</param>
            /// <param name="inner">The inner.</param>
            public ModbusException(string message, Exception inner)
                : base(message, inner) {
            }

            /// <summary>
            /// Gets the modbus exception collection.
            /// </summary>
            /// <value>The modbus exception collection.</value>
            private static IEnumerable<ModbusException> ModbusExceptionCollection {
                get {
                    if (_modbusExceptionCollection == null) {
                        _modbusExceptionCollection = new List<ModbusException>();
                        _modbusExceptionCollection.Add(new ModbusException("ILLEGAL FUNCTION") {
                            Code = 0x01,
                            Description =
                                "The function code received in the query is not an allowable action for the server. This may be because the function code is only applicable to newer devices, and was not implemented in the unit selected. It could also indicate that the serveris in the wrong state to process a request of this type, for example because it is unconfigured and is being asked to return register values."
                        });
                        _modbusExceptionCollection.Add(new ModbusException("ILLEGAL DATA ADDRESS") {
                            Code = 0x02,
                            Description =
                                "The data address received in the query is not an allowable address for the  server. More specifically, the combination of reference number and transfer length is invalid. For a controller with 100 registers, the PDU addresses the first register as 0, and the last one as 99. If a request is submitted with a starting register address of 96 and a quantity of registers of 4, then this request will successfully operate (address-wise at least) on registers 96, 97, 98, 99. If a request is submitted with a starting register address of 96 and a quantity of registers of 5, then this request will fail with Exception Code 0x02 “Illegal Data Address” since it attempts to operate on registers 96, 97, 98, 99 and 100, and there is no register with address 100."
                        });
                        _modbusExceptionCollection.Add(new ModbusException("ILLEGAL DATA VALUE") {
                            Code = 0x03,
                            Description =
                                "A value contained in the query data field is not an allowable value for server. This indicates a fault in the structure of the remainder of a complex request, such as that the implied length is incorrect. It specifically does NOT mean that a data item submitted for storage in a register has a value outside the expectation of the application program, since the MODBUS protocol is unaware of the significance of any particular value of any particular register."
                        });
                        _modbusExceptionCollection.Add(new ModbusException("SERVER DEVICE FAILURE") {
                            Code = 0x04,
                            Description =
                                "An unrecoverable error occurred while the serverwas attempting to perform the requested action."
                        });
                        _modbusExceptionCollection.Add(new ModbusException("ACKNOWLEDGE") {
                            Code = 0x05,
                            Description =
                                "Specialized use in conjunction with programming commands.The server  has accepted the request and is processing it, but a long duration of time will be required to do so. This response is returned to prevent a timeout error from occurring in the client. The client can next issue a Poll Program Complete message to determine if processing is completed."
                        });
                        _modbusExceptionCollection.Add(new ModbusException("SERVER DEVICE BUSY") {
                            Code = 0x06,
                            Description =
                                "Specialized use in conjunction with programming commands.The server  is engaged in processing a long–duration program command. The client  should retransmit the message later when the server is free."
                        });
                        _modbusExceptionCollection.Add(new ModbusException("MEMORY PARITY ERROR") {
                            Code = 0x08,
                            Description =
                                "Specialized use in conjunction with function codes 20 and 21 and reference type 6, to indicate that the extended file area failed to pass a consistency check.The server  attempted to read record file, but detected a parity error in the memory. The client can retry the request, but service may be required MODBUS Application Protocol Specification V1.1b3  ModbusApril 26, 2012  http://www.modbus.org  49/50on the server device."
                        });
                        _modbusExceptionCollection.Add(new ModbusException("GATEWAY PATH UNAVAILABLE") {
                            Code = 0x0A,
                            Description =
                                "Specialized use in conjunction with gateways,indicates that the gateway was unable to allocate an internal communication path from the input port to the output port for processing the request. Usually means that the gateway is misconfigured or overloaded."
                        });
                        _modbusExceptionCollection.Add(new ModbusException("GATEWAY TARGET DEVICE FAILED TO RESPOND") {
                            Code = 0x0B,
                            Description =
                                "Specialized use in conjunction with gateways, indicates that no response was obtained from the target device. Usually means that the device is not present on the network."
                        });
                    }
                    return _modbusExceptionCollection;
                }
            }

            /// <summary>
            /// Gets the modbus exception.
            /// </summary>
            /// <param name="code">The code.</param>
            /// <returns>ModbusException.</returns>
            public static ModbusException GetModbusException(byte code) {
                var query = (from ex in ModbusException.ModbusExceptionCollection
                             where ex.Code == code
                             select ex).FirstOrDefault();

                return query;
            }

            //public override string Message
            //{
            //    get
            //    {
            //        if (this.Code == null)
            //        {
            //            return base.Message;
            //        }
            //        else
            //        {
            //            return string.Format("Code:{0}, Message:{1}", this.Code, base.Message);
            //        }
            //    }
            //}
        }

        //[Serializable]
        //public class ModbusException : Exception
        //{
        //    private static List<ModbusException> s_ModbusExceptionCollection;

        //    public virtual byte Code { get; set; }

        //    public virtual string Description { get; set; }

        //    public ModbusException()
        //        : base("show message") { }

        //    public ModbusException(string message)
        //        : base(message) { }

        //    public ModbusException(string message, Exception inner)
        //        : base(message, inner) { }

        //    protected ModbusException(SerializationInfo info, StreamingContext context)
        //        : base(info, context) { }

        //    private static IEnumerable<ModbusException> ModbusExceptionCollection
        //    {
        //        get
        //        {
        //            if (s_ModbusExceptionCollection == null)
        //            {
        //                s_ModbusExceptionCollection = new List<ModbusException>();
        //                s_ModbusExceptionCollection.Add(new ModbusException("ILLEGAL FUNCTION") { Code = 0x01, Description = "The function code received in the query is not an allowable action for the server. This may be because the function code is only applicable to newer devices, and was not implemented in the unit selected. It could also indicate that the serveris in the wrong state to process a request of this type, for example because it is unconfigured and is being asked to return register values." });
        //                s_ModbusExceptionCollection.Add(new ModbusException("ILLEGAL DATA ADDRESS") { Code = 0x02, Description = "The data address received in the query is not an allowable address for the  server. More specifically, the combination of reference number and transfer length is invalid. For a controller with 100 registers, the PDU addresses the first register as 0, and the last one as 99. If a request is submitted with a starting register address of 96 and a quantity of registers of 4, then this request will successfully operate (address-wise at least) on registers 96, 97, 98, 99. If a request is submitted with a starting register address of 96 and a quantity of registers of 5, then this request will fail with Exception Code 0x02 “Illegal Data Address” since it attempts to operate on registers 96, 97, 98, 99 and 100, and there is no register with address 100." });
        //                s_ModbusExceptionCollection.Add(new ModbusException("ILLEGAL DATA VALUE") { Code = 0x03, Description = "A value contained in the query data field is not an allowable value for server. This indicates a fault in the structure of the remainder of a complex request, such as that the implied length is incorrect. It specifically does NOT mean that a data item submitted for storage in a register has a value outside the expectation of the application program, since the MODBUS protocol is unaware of the significance of any particular value of any particular register." });
        //                s_ModbusExceptionCollection.Add(new ModbusException("SERVER DEVICE FAILURE") { Code = 0x04, Description = "An unrecoverable error occurred while the serverwas attempting to perform the requested action." });
        //                s_ModbusExceptionCollection.Add(new ModbusException("ACKNOWLEDGE") { Code = 0x05, Description = "Specialized use in conjunction with programming commands.The server  has accepted the request and is processing it, but a long duration of time will be required to do so. This response is returned to prevent a timeout error from occurring in the client. The client can next issue a Poll Program Complete message to determine if processing is completed." });
        //                s_ModbusExceptionCollection.Add(new ModbusException("SERVER DEVICE BUSY") { Code = 0x06, Description = "Specialized use in conjunction with programming commands.The server  is engaged in processing a long–duration program command. The client  should retransmit the message later when the server is free." });
        //                s_ModbusExceptionCollection.Add(new ModbusException("MEMORY PARITY ERROR") { Code = 0x08, Description = "Specialized use in conjunction with function codes 20 and 21 and reference type 6, to indicate that the extended file area failed to pass a consistency check.The server  attempted to read record file, but detected a parity error in the memory. The client can retry the request, but service may be required MODBUS Application Protocol Specification V1.1b3  ModbusApril 26, 2012  http://www.modbus.org  49/50on the server device." });
        //                s_ModbusExceptionCollection.Add(new ModbusException("GATEWAY PATH UNAVAILABLE") { Code = 0x0A, Description = "Specialized use in conjunction with gateways,indicates that the gateway was unable to allocate an internal communication path from the input port to the output port for processing the request. Usually means that the gateway is misconfigured or overloaded." });
        //                s_ModbusExceptionCollection.Add(new ModbusException("GATEWAY TARGET DEVICE FAILED TO RESPOND") { Code = 0x0B, Description = "Specialized use in conjunction with gateways, indicates that no response was obtained from the target device. Usually means that the device is not present on the network." });
        //            }
        //            return s_ModbusExceptionCollection;
        //        }
        //    }

        //    public static ModbusException GetModbusException(byte Code)
        //    {
        //        var query = (from ex in ModbusException.ModbusExceptionCollection
        //                     where ex.Code == Code
        //                     select ex).FirstOrDefault();

        //        return query;
        //    }

        //    public override string Message
        //    {
        //        get
        //        {
        //            return base.Message;
        //        }
        //    }

        //}
    }
