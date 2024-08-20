using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

public partial class MAVLink
{
    public const string MAVLINK_BUILD_DATE = "Tue Aug 20 2024";
    public const string MAVLINK_WIRE_PROTOCOL_VERSION = "2.0";
    public const int MAVLINK_MAX_PAYLOAD_LEN = 80;

    public const byte MAVLINK_CORE_HEADER_LEN = 9;///< Length of core header (of the comm. layer)
    public const byte MAVLINK_CORE_HEADER_MAVLINK1_LEN = 5;///< Length of MAVLink1 core header (of the comm. layer)
    public const byte MAVLINK_NUM_HEADER_BYTES = (MAVLINK_CORE_HEADER_LEN + 1);///< Length of all header bytes, including core and stx
    public const byte MAVLINK_NUM_CHECKSUM_BYTES = 2;
    public const byte MAVLINK_NUM_NON_PAYLOAD_BYTES = (MAVLINK_NUM_HEADER_BYTES + MAVLINK_NUM_CHECKSUM_BYTES);

    public const int MAVLINK_MAX_PACKET_LEN = (MAVLINK_MAX_PAYLOAD_LEN + MAVLINK_NUM_NON_PAYLOAD_BYTES + MAVLINK_SIGNATURE_BLOCK_LEN);///< Maximum packet length
    public const byte MAVLINK_SIGNATURE_BLOCK_LEN = 13;

    public const int MAVLINK_LITTLE_ENDIAN = 1;
    public const int MAVLINK_BIG_ENDIAN = 0;

    public const byte MAVLINK_STX = 253;

    public const byte MAVLINK_STX_MAVLINK1 = 0xFE;

    public const byte MAVLINK_ENDIAN = MAVLINK_LITTLE_ENDIAN;

    public const bool MAVLINK_ALIGNED_FIELDS = (1 == 1);

    public const byte MAVLINK_CRC_EXTRA = 1;
    
    public const byte MAVLINK_COMMAND_24BIT = 1;
        
    public const bool MAVLINK_NEED_BYTE_SWAP = (MAVLINK_ENDIAN == MAVLINK_LITTLE_ENDIAN);
        
    // msgid, name, crc, minlength, length, type
    public static message_info[] MAVLINK_MESSAGE_INFOS = new message_info[] {
        new message_info(31, "ATTITUDE_QUATERNION", 219, 64, 80, typeof( mavlink_attitude_quaternion_t )),

    };

    public const byte MAVLINK_VERSION = 3;

    public const byte MAVLINK_IFLAG_SIGNED=  0x01;
    public const byte MAVLINK_IFLAG_MASK   = 0x01;

    public struct message_info
    {
        public uint msgid { get; internal set; }
        public string name { get; internal set; }
        public byte crc { get; internal set; }
        public uint minlength { get; internal set; }
        public uint length { get; internal set; }
        public Type type { get; internal set; }

        public message_info(uint msgid, string name, byte crc, uint minlength, uint length, Type type)
        {
            this.msgid = msgid;
            this.name = name;
            this.crc = crc;
            this.minlength = minlength;
            this.length = length;
            this.type = type;
        }

        public override string ToString()
        {
            return String.Format("{0} - {1}",name,msgid);
        }
    }   

    public enum MAVLINK_MSG_ID 
    {

        ATTITUDE_QUATERNION = 31,
    }
    
    
    
    /// extensions_start 16
    [StructLayout(LayoutKind.Sequential,Pack=1,Size=80)]
    ///<summary> The attitude in the aeronautical frame (right-handed, Z-down, X-front, Y-right),         expressed as quaternion. Quaternion order is w, x, y, z and a zero rotation would be         expressed as (1 0 0 0). </summary>
    public struct mavlink_attitude_quaternion_t
    {
        public mavlink_attitude_quaternion_t(float Airspeed,float Altimeter,float Gyroscope_X_axis,float Gyroscope_Y_axis,float Gyroscope_Z_axis,float Gyroscope_W_axis,float GPS_X_axis,float GPS_Y_axis,float total_energy,float energy_loss_rate,float efficiency,float thermometer,float barometer,float wind_direction_X_axis,float wind_direction_Y_axis,float wind_direction_Z_axis,float[] repr_offset_q) 
        {
              this.Airspeed = Airspeed;
              this.Altimeter = Altimeter;
              this.Gyroscope_X_axis = Gyroscope_X_axis;
              this.Gyroscope_Y_axis = Gyroscope_Y_axis;
              this.Gyroscope_Z_axis = Gyroscope_Z_axis;
              this.Gyroscope_W_axis = Gyroscope_W_axis;
              this.GPS_X_axis = GPS_X_axis;
              this.GPS_Y_axis = GPS_Y_axis;
              this.total_energy = total_energy;
              this.energy_loss_rate = energy_loss_rate;
              this.efficiency = efficiency;
              this.thermometer = thermometer;
              this.barometer = barometer;
              this.wind_direction_X_axis = wind_direction_X_axis;
              this.wind_direction_Y_axis = wind_direction_Y_axis;
              this.wind_direction_Z_axis = wind_direction_Z_axis;
              this.repr_offset_q = repr_offset_q;
            
        }
        /// <summary> Airspeed  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description(" Airspeed")]
        public  float Airspeed;
            /// <summary>Altitude  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Altitude")]
        public  float Altimeter;
            /// <summary>Gyroscope in the X dimension  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Gyroscope in the X dimension")]
        public  float Gyroscope_X_axis;
            /// <summary>Gyroscope in the Y dimension  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Gyroscope in the Y dimension")]
        public  float Gyroscope_Y_axis;
            /// <summary>Gyroscope in the Z dimension  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Gyroscope in the Z dimension")]
        public  float Gyroscope_Z_axis;
            /// <summary>Scalar of the Gyroscope  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Scalar of the Gyroscope")]
        public  float Gyroscope_W_axis;
            /// <summary>GPS in the X axis  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("GPS in the X axis")]
        public  float GPS_X_axis;
            /// <summary>GPS in the Y axis  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("GPS in the Y axis")]
        public  float GPS_Y_axis;
            /// <summary>Total energy of Kinetic and Potential         Energy added together  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Total energy of Kinetic and Potential         Energy added together")]
        public  float total_energy;
            /// <summary>Loss of total energy  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Loss of total energy")]
        public  float energy_loss_rate;
            /// <summary>Efficiency of system  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Efficiency of system")]
        public  float efficiency;
            /// <summary>Measures temperature  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Measures temperature")]
        public  float thermometer;
            /// <summary>Measures pressure  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Measures pressure")]
        public  float barometer;
            /// <summary>Direction of wind in X axis  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Direction of wind in X axis")]
        public  float wind_direction_X_axis;
            /// <summary>Direction of wind in Y axis  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Direction of wind in Y axis")]
        public  float wind_direction_Y_axis;
            /// <summary>Direction of wind in Z axis  [rad/s] </summary>
        [Units("[rad/s]")]
        [Description("Direction of wind in Z axis")]
        public  float wind_direction_Z_axis;
            /// <summary>Rotation offset by which the attitude quaternion         and angular speed vector should be rotated for user display (quaternion with [w, x, y, z]         order, zero-rotation is [1, 0, 0, 0], send [0, 0, 0, 0] if field not supported). This field         is intended for systems in which the reference attitude may change during flight. For         example, tailsitters VTOLs rotate their reference attitude by 90 degrees between hover mode         and fixed wing mode, thus repr_offset_q is equal to [1, 0, 0, 0] in hover mode and equal to         [0.7071, 0, 0.7071, 0] in fixed wing mode.   </summary>
        [Units("")]
        [Description("Rotation offset by which the attitude quaternion         and angular speed vector should be rotated for user display (quaternion with [w, x, y, z]         order, zero-rotation is [1, 0, 0, 0], send [0, 0, 0, 0] if field not supported). This field         is intended for systems in which the reference attitude may change during flight. For         example, tailsitters VTOLs rotate their reference attitude by 90 degrees between hover mode         and fixed wing mode, thus repr_offset_q is equal to [1, 0, 0, 0] in hover mode and equal to         [0.7071, 0, 0.7071, 0] in fixed wing mode.")]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public float[] repr_offset_q;
    
    };

}
