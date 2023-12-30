#pragma once
// MESSAGE ATTITUDE_QUATERNION PACKING

#define MAVLINK_MSG_ID_ATTITUDE_QUATERNION 31


typedef struct __mavlink_attitude_quaternion_t {
 float Airspeed; /*< [rad/s]  Airspeed*/
 float Altimeter; /*< [rad/s] Altitude*/
 float Gyroscope_X_axis; /*< [rad/s] Gyroscope in the X dimension*/
 float Gyroscope_Y_axis; /*< [rad/s] Gyroscope in the Y dimension*/
 float Gyroscope_Z_axis; /*< [rad/s] Gyroscope in the Z dimension*/
 float Gyroscope_W_axis; /*< [rad/s] Scalar of the Gyroscope*/
 float GPS_X_axis; /*< [rad/s] GPS in the X axis*/
 float GPS_Y_axis; /*< [rad/s] GPS in the Y axis*/
 float total_energy; /*< [rad/s] Total energy of Kinetic and Potential
        Energy added together*/
 float energy_loss_rate; /*< [rad/s] Loss of total energy*/
 float efficiency; /*< [rad/s] Efficiency of system*/
 float thermometer; /*< [rad/s] Measures temperature*/
 float barometer; /*< [rad/s] Measures pressure*/
 float wind_direction_X_axis; /*< [rad/s] Direction of wind in X axis*/
 float wind_direction_Y_axis; /*< [rad/s] Direction of wind in Y axis*/
 float wind_direction_Z_axis; /*< [rad/s] Direction of wind in Z axis*/
 float repr_offset_q[4]; /*<  Rotation offset by which the attitude quaternion
        and angular speed vector should be rotated for user display (quaternion with [w, x, y, z]
        order, zero-rotation is [1, 0, 0, 0], send [0, 0, 0, 0] if field not supported). This field
        is intended for systems in which the reference attitude may change during flight. For
        example, tailsitters VTOLs rotate their reference attitude by 90 degrees between hover mode
        and fixed wing mode, thus repr_offset_q is equal to [1, 0, 0, 0] in hover mode and equal to
        [0.7071, 0, 0.7071, 0] in fixed wing mode.*/
} mavlink_attitude_quaternion_t;

#define MAVLINK_MSG_ID_ATTITUDE_QUATERNION_LEN 80
#define MAVLINK_MSG_ID_ATTITUDE_QUATERNION_MIN_LEN 64
#define MAVLINK_MSG_ID_31_LEN 80
#define MAVLINK_MSG_ID_31_MIN_LEN 64

#define MAVLINK_MSG_ID_ATTITUDE_QUATERNION_CRC 219
#define MAVLINK_MSG_ID_31_CRC 219

#define MAVLINK_MSG_ATTITUDE_QUATERNION_FIELD_REPR_OFFSET_Q_LEN 4

#if MAVLINK_COMMAND_24BIT
#define MAVLINK_MESSAGE_INFO_ATTITUDE_QUATERNION { \
    31, \
    "ATTITUDE_QUATERNION", \
    17, \
    {  { "Airspeed", NULL, MAVLINK_TYPE_FLOAT, 0, 0, offsetof(mavlink_attitude_quaternion_t, Airspeed) }, \
         { "Altimeter", NULL, MAVLINK_TYPE_FLOAT, 0, 4, offsetof(mavlink_attitude_quaternion_t, Altimeter) }, \
         { "Gyroscope_X_axis", NULL, MAVLINK_TYPE_FLOAT, 0, 8, offsetof(mavlink_attitude_quaternion_t, Gyroscope_X_axis) }, \
         { "Gyroscope_Y_axis", NULL, MAVLINK_TYPE_FLOAT, 0, 12, offsetof(mavlink_attitude_quaternion_t, Gyroscope_Y_axis) }, \
         { "Gyroscope_Z_axis", NULL, MAVLINK_TYPE_FLOAT, 0, 16, offsetof(mavlink_attitude_quaternion_t, Gyroscope_Z_axis) }, \
         { "Gyroscope_W_axis", NULL, MAVLINK_TYPE_FLOAT, 0, 20, offsetof(mavlink_attitude_quaternion_t, Gyroscope_W_axis) }, \
         { "GPS_X_axis", NULL, MAVLINK_TYPE_FLOAT, 0, 24, offsetof(mavlink_attitude_quaternion_t, GPS_X_axis) }, \
         { "GPS_Y_axis", NULL, MAVLINK_TYPE_FLOAT, 0, 28, offsetof(mavlink_attitude_quaternion_t, GPS_Y_axis) }, \
         { "total_energy", NULL, MAVLINK_TYPE_FLOAT, 0, 32, offsetof(mavlink_attitude_quaternion_t, total_energy) }, \
         { "energy_loss_rate", NULL, MAVLINK_TYPE_FLOAT, 0, 36, offsetof(mavlink_attitude_quaternion_t, energy_loss_rate) }, \
         { "efficiency", NULL, MAVLINK_TYPE_FLOAT, 0, 40, offsetof(mavlink_attitude_quaternion_t, efficiency) }, \
         { "thermometer", NULL, MAVLINK_TYPE_FLOAT, 0, 44, offsetof(mavlink_attitude_quaternion_t, thermometer) }, \
         { "barometer", NULL, MAVLINK_TYPE_FLOAT, 0, 48, offsetof(mavlink_attitude_quaternion_t, barometer) }, \
         { "wind_direction_X_axis", NULL, MAVLINK_TYPE_FLOAT, 0, 52, offsetof(mavlink_attitude_quaternion_t, wind_direction_X_axis) }, \
         { "wind_direction_Y_axis", NULL, MAVLINK_TYPE_FLOAT, 0, 56, offsetof(mavlink_attitude_quaternion_t, wind_direction_Y_axis) }, \
         { "wind_direction_Z_axis", NULL, MAVLINK_TYPE_FLOAT, 0, 60, offsetof(mavlink_attitude_quaternion_t, wind_direction_Z_axis) }, \
         { "repr_offset_q", NULL, MAVLINK_TYPE_FLOAT, 4, 64, offsetof(mavlink_attitude_quaternion_t, repr_offset_q) }, \
         } \
}
#else
#define MAVLINK_MESSAGE_INFO_ATTITUDE_QUATERNION { \
    "ATTITUDE_QUATERNION", \
    17, \
    {  { "Airspeed", NULL, MAVLINK_TYPE_FLOAT, 0, 0, offsetof(mavlink_attitude_quaternion_t, Airspeed) }, \
         { "Altimeter", NULL, MAVLINK_TYPE_FLOAT, 0, 4, offsetof(mavlink_attitude_quaternion_t, Altimeter) }, \
         { "Gyroscope_X_axis", NULL, MAVLINK_TYPE_FLOAT, 0, 8, offsetof(mavlink_attitude_quaternion_t, Gyroscope_X_axis) }, \
         { "Gyroscope_Y_axis", NULL, MAVLINK_TYPE_FLOAT, 0, 12, offsetof(mavlink_attitude_quaternion_t, Gyroscope_Y_axis) }, \
         { "Gyroscope_Z_axis", NULL, MAVLINK_TYPE_FLOAT, 0, 16, offsetof(mavlink_attitude_quaternion_t, Gyroscope_Z_axis) }, \
         { "Gyroscope_W_axis", NULL, MAVLINK_TYPE_FLOAT, 0, 20, offsetof(mavlink_attitude_quaternion_t, Gyroscope_W_axis) }, \
         { "GPS_X_axis", NULL, MAVLINK_TYPE_FLOAT, 0, 24, offsetof(mavlink_attitude_quaternion_t, GPS_X_axis) }, \
         { "GPS_Y_axis", NULL, MAVLINK_TYPE_FLOAT, 0, 28, offsetof(mavlink_attitude_quaternion_t, GPS_Y_axis) }, \
         { "total_energy", NULL, MAVLINK_TYPE_FLOAT, 0, 32, offsetof(mavlink_attitude_quaternion_t, total_energy) }, \
         { "energy_loss_rate", NULL, MAVLINK_TYPE_FLOAT, 0, 36, offsetof(mavlink_attitude_quaternion_t, energy_loss_rate) }, \
         { "efficiency", NULL, MAVLINK_TYPE_FLOAT, 0, 40, offsetof(mavlink_attitude_quaternion_t, efficiency) }, \
         { "thermometer", NULL, MAVLINK_TYPE_FLOAT, 0, 44, offsetof(mavlink_attitude_quaternion_t, thermometer) }, \
         { "barometer", NULL, MAVLINK_TYPE_FLOAT, 0, 48, offsetof(mavlink_attitude_quaternion_t, barometer) }, \
         { "wind_direction_X_axis", NULL, MAVLINK_TYPE_FLOAT, 0, 52, offsetof(mavlink_attitude_quaternion_t, wind_direction_X_axis) }, \
         { "wind_direction_Y_axis", NULL, MAVLINK_TYPE_FLOAT, 0, 56, offsetof(mavlink_attitude_quaternion_t, wind_direction_Y_axis) }, \
         { "wind_direction_Z_axis", NULL, MAVLINK_TYPE_FLOAT, 0, 60, offsetof(mavlink_attitude_quaternion_t, wind_direction_Z_axis) }, \
         { "repr_offset_q", NULL, MAVLINK_TYPE_FLOAT, 4, 64, offsetof(mavlink_attitude_quaternion_t, repr_offset_q) }, \
         } \
}
#endif

/**
 * @brief Pack a attitude_quaternion message
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param msg The MAVLink message to compress the data into
 *
 * @param Airspeed [rad/s]  Airspeed
 * @param Altimeter [rad/s] Altitude
 * @param Gyroscope_X_axis [rad/s] Gyroscope in the X dimension
 * @param Gyroscope_Y_axis [rad/s] Gyroscope in the Y dimension
 * @param Gyroscope_Z_axis [rad/s] Gyroscope in the Z dimension
 * @param Gyroscope_W_axis [rad/s] Scalar of the Gyroscope
 * @param GPS_X_axis [rad/s] GPS in the X axis
 * @param GPS_Y_axis [rad/s] GPS in the Y axis
 * @param total_energy [rad/s] Total energy of Kinetic and Potential
        Energy added together
 * @param energy_loss_rate [rad/s] Loss of total energy
 * @param efficiency [rad/s] Efficiency of system
 * @param thermometer [rad/s] Measures temperature
 * @param barometer [rad/s] Measures pressure
 * @param wind_direction_X_axis [rad/s] Direction of wind in X axis
 * @param wind_direction_Y_axis [rad/s] Direction of wind in Y axis
 * @param wind_direction_Z_axis [rad/s] Direction of wind in Z axis
 * @param repr_offset_q  Rotation offset by which the attitude quaternion
        and angular speed vector should be rotated for user display (quaternion with [w, x, y, z]
        order, zero-rotation is [1, 0, 0, 0], send [0, 0, 0, 0] if field not supported). This field
        is intended for systems in which the reference attitude may change during flight. For
        example, tailsitters VTOLs rotate their reference attitude by 90 degrees between hover mode
        and fixed wing mode, thus repr_offset_q is equal to [1, 0, 0, 0] in hover mode and equal to
        [0.7071, 0, 0.7071, 0] in fixed wing mode.
 * @return length of the message in bytes (excluding serial stream start sign)
 */
static inline uint16_t mavlink_msg_attitude_quaternion_pack(uint8_t system_id, uint8_t component_id, mavlink_message_t* msg,
                               float Airspeed, float Altimeter, float Gyroscope_X_axis, float Gyroscope_Y_axis, float Gyroscope_Z_axis, float Gyroscope_W_axis, float GPS_X_axis, float GPS_Y_axis, float total_energy, float energy_loss_rate, float efficiency, float thermometer, float barometer, float wind_direction_X_axis, float wind_direction_Y_axis, float wind_direction_Z_axis, const float *repr_offset_q)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char buf[MAVLINK_MSG_ID_ATTITUDE_QUATERNION_LEN];
    _mav_put_float(buf, 0, Airspeed);
    _mav_put_float(buf, 4, Altimeter);
    _mav_put_float(buf, 8, Gyroscope_X_axis);
    _mav_put_float(buf, 12, Gyroscope_Y_axis);
    _mav_put_float(buf, 16, Gyroscope_Z_axis);
    _mav_put_float(buf, 20, Gyroscope_W_axis);
    _mav_put_float(buf, 24, GPS_X_axis);
    _mav_put_float(buf, 28, GPS_Y_axis);
    _mav_put_float(buf, 32, total_energy);
    _mav_put_float(buf, 36, energy_loss_rate);
    _mav_put_float(buf, 40, efficiency);
    _mav_put_float(buf, 44, thermometer);
    _mav_put_float(buf, 48, barometer);
    _mav_put_float(buf, 52, wind_direction_X_axis);
    _mav_put_float(buf, 56, wind_direction_Y_axis);
    _mav_put_float(buf, 60, wind_direction_Z_axis);
    _mav_put_float_array(buf, 64, repr_offset_q, 4);
        memcpy(_MAV_PAYLOAD_NON_CONST(msg), buf, MAVLINK_MSG_ID_ATTITUDE_QUATERNION_LEN);
#else
    mavlink_attitude_quaternion_t packet;
    packet.Airspeed = Airspeed;
    packet.Altimeter = Altimeter;
    packet.Gyroscope_X_axis = Gyroscope_X_axis;
    packet.Gyroscope_Y_axis = Gyroscope_Y_axis;
    packet.Gyroscope_Z_axis = Gyroscope_Z_axis;
    packet.Gyroscope_W_axis = Gyroscope_W_axis;
    packet.GPS_X_axis = GPS_X_axis;
    packet.GPS_Y_axis = GPS_Y_axis;
    packet.total_energy = total_energy;
    packet.energy_loss_rate = energy_loss_rate;
    packet.efficiency = efficiency;
    packet.thermometer = thermometer;
    packet.barometer = barometer;
    packet.wind_direction_X_axis = wind_direction_X_axis;
    packet.wind_direction_Y_axis = wind_direction_Y_axis;
    packet.wind_direction_Z_axis = wind_direction_Z_axis;
    mav_array_memcpy(packet.repr_offset_q, repr_offset_q, sizeof(float)*4);
        memcpy(_MAV_PAYLOAD_NON_CONST(msg), &packet, MAVLINK_MSG_ID_ATTITUDE_QUATERNION_LEN);
#endif

    msg->msgid = MAVLINK_MSG_ID_ATTITUDE_QUATERNION;
    return mavlink_finalize_message(msg, system_id, component_id, MAVLINK_MSG_ID_ATTITUDE_QUATERNION_MIN_LEN, MAVLINK_MSG_ID_ATTITUDE_QUATERNION_LEN, MAVLINK_MSG_ID_ATTITUDE_QUATERNION_CRC);
}

/**
 * @brief Pack a attitude_quaternion message
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param status MAVLink status structure
 * @param msg The MAVLink message to compress the data into
 *
 * @param Airspeed [rad/s]  Airspeed
 * @param Altimeter [rad/s] Altitude
 * @param Gyroscope_X_axis [rad/s] Gyroscope in the X dimension
 * @param Gyroscope_Y_axis [rad/s] Gyroscope in the Y dimension
 * @param Gyroscope_Z_axis [rad/s] Gyroscope in the Z dimension
 * @param Gyroscope_W_axis [rad/s] Scalar of the Gyroscope
 * @param GPS_X_axis [rad/s] GPS in the X axis
 * @param GPS_Y_axis [rad/s] GPS in the Y axis
 * @param total_energy [rad/s] Total energy of Kinetic and Potential
        Energy added together
 * @param energy_loss_rate [rad/s] Loss of total energy
 * @param efficiency [rad/s] Efficiency of system
 * @param thermometer [rad/s] Measures temperature
 * @param barometer [rad/s] Measures pressure
 * @param wind_direction_X_axis [rad/s] Direction of wind in X axis
 * @param wind_direction_Y_axis [rad/s] Direction of wind in Y axis
 * @param wind_direction_Z_axis [rad/s] Direction of wind in Z axis
 * @param repr_offset_q  Rotation offset by which the attitude quaternion
        and angular speed vector should be rotated for user display (quaternion with [w, x, y, z]
        order, zero-rotation is [1, 0, 0, 0], send [0, 0, 0, 0] if field not supported). This field
        is intended for systems in which the reference attitude may change during flight. For
        example, tailsitters VTOLs rotate their reference attitude by 90 degrees between hover mode
        and fixed wing mode, thus repr_offset_q is equal to [1, 0, 0, 0] in hover mode and equal to
        [0.7071, 0, 0.7071, 0] in fixed wing mode.
 * @return length of the message in bytes (excluding serial stream start sign)
 */
static inline uint16_t mavlink_msg_attitude_quaternion_pack_status(uint8_t system_id, uint8_t component_id, mavlink_status_t *_status, mavlink_message_t* msg,
                               float Airspeed, float Altimeter, float Gyroscope_X_axis, float Gyroscope_Y_axis, float Gyroscope_Z_axis, float Gyroscope_W_axis, float GPS_X_axis, float GPS_Y_axis, float total_energy, float energy_loss_rate, float efficiency, float thermometer, float barometer, float wind_direction_X_axis, float wind_direction_Y_axis, float wind_direction_Z_axis, const float *repr_offset_q)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char buf[MAVLINK_MSG_ID_ATTITUDE_QUATERNION_LEN];
    _mav_put_float(buf, 0, Airspeed);
    _mav_put_float(buf, 4, Altimeter);
    _mav_put_float(buf, 8, Gyroscope_X_axis);
    _mav_put_float(buf, 12, Gyroscope_Y_axis);
    _mav_put_float(buf, 16, Gyroscope_Z_axis);
    _mav_put_float(buf, 20, Gyroscope_W_axis);
    _mav_put_float(buf, 24, GPS_X_axis);
    _mav_put_float(buf, 28, GPS_Y_axis);
    _mav_put_float(buf, 32, total_energy);
    _mav_put_float(buf, 36, energy_loss_rate);
    _mav_put_float(buf, 40, efficiency);
    _mav_put_float(buf, 44, thermometer);
    _mav_put_float(buf, 48, barometer);
    _mav_put_float(buf, 52, wind_direction_X_axis);
    _mav_put_float(buf, 56, wind_direction_Y_axis);
    _mav_put_float(buf, 60, wind_direction_Z_axis);
    _mav_put_float_array(buf, 64, repr_offset_q, 4);
        memcpy(_MAV_PAYLOAD_NON_CONST(msg), buf, MAVLINK_MSG_ID_ATTITUDE_QUATERNION_LEN);
#else
    mavlink_attitude_quaternion_t packet;
    packet.Airspeed = Airspeed;
    packet.Altimeter = Altimeter;
    packet.Gyroscope_X_axis = Gyroscope_X_axis;
    packet.Gyroscope_Y_axis = Gyroscope_Y_axis;
    packet.Gyroscope_Z_axis = Gyroscope_Z_axis;
    packet.Gyroscope_W_axis = Gyroscope_W_axis;
    packet.GPS_X_axis = GPS_X_axis;
    packet.GPS_Y_axis = GPS_Y_axis;
    packet.total_energy = total_energy;
    packet.energy_loss_rate = energy_loss_rate;
    packet.efficiency = efficiency;
    packet.thermometer = thermometer;
    packet.barometer = barometer;
    packet.wind_direction_X_axis = wind_direction_X_axis;
    packet.wind_direction_Y_axis = wind_direction_Y_axis;
    packet.wind_direction_Z_axis = wind_direction_Z_axis;
    mav_array_memcpy(packet.repr_offset_q, repr_offset_q, sizeof(float)*4);
        memcpy(_MAV_PAYLOAD_NON_CONST(msg), &packet, MAVLINK_MSG_ID_ATTITUDE_QUATERNION_LEN);
#endif

    msg->msgid = MAVLINK_MSG_ID_ATTITUDE_QUATERNION;
#if MAVLINK_CRC_EXTRA
    return mavlink_finalize_message_buffer(msg, system_id, component_id, _status, MAVLINK_MSG_ID_ATTITUDE_QUATERNION_MIN_LEN, MAVLINK_MSG_ID_ATTITUDE_QUATERNION_LEN, MAVLINK_MSG_ID_ATTITUDE_QUATERNION_CRC);
#else
    return mavlink_finalize_message_buffer(msg, system_id, component_id, _status, MAVLINK_MSG_ID_ATTITUDE_QUATERNION_MIN_LEN, MAVLINK_MSG_ID_ATTITUDE_QUATERNION_LEN);
#endif
}

/**
 * @brief Pack a attitude_quaternion message on a channel
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param chan The MAVLink channel this message will be sent over
 * @param msg The MAVLink message to compress the data into
 * @param Airspeed [rad/s]  Airspeed
 * @param Altimeter [rad/s] Altitude
 * @param Gyroscope_X_axis [rad/s] Gyroscope in the X dimension
 * @param Gyroscope_Y_axis [rad/s] Gyroscope in the Y dimension
 * @param Gyroscope_Z_axis [rad/s] Gyroscope in the Z dimension
 * @param Gyroscope_W_axis [rad/s] Scalar of the Gyroscope
 * @param GPS_X_axis [rad/s] GPS in the X axis
 * @param GPS_Y_axis [rad/s] GPS in the Y axis
 * @param total_energy [rad/s] Total energy of Kinetic and Potential
        Energy added together
 * @param energy_loss_rate [rad/s] Loss of total energy
 * @param efficiency [rad/s] Efficiency of system
 * @param thermometer [rad/s] Measures temperature
 * @param barometer [rad/s] Measures pressure
 * @param wind_direction_X_axis [rad/s] Direction of wind in X axis
 * @param wind_direction_Y_axis [rad/s] Direction of wind in Y axis
 * @param wind_direction_Z_axis [rad/s] Direction of wind in Z axis
 * @param repr_offset_q  Rotation offset by which the attitude quaternion
        and angular speed vector should be rotated for user display (quaternion with [w, x, y, z]
        order, zero-rotation is [1, 0, 0, 0], send [0, 0, 0, 0] if field not supported). This field
        is intended for systems in which the reference attitude may change during flight. For
        example, tailsitters VTOLs rotate their reference attitude by 90 degrees between hover mode
        and fixed wing mode, thus repr_offset_q is equal to [1, 0, 0, 0] in hover mode and equal to
        [0.7071, 0, 0.7071, 0] in fixed wing mode.
 * @return length of the message in bytes (excluding serial stream start sign)
 */
static inline uint16_t mavlink_msg_attitude_quaternion_pack_chan(uint8_t system_id, uint8_t component_id, uint8_t chan,
                               mavlink_message_t* msg,
                                   float Airspeed,float Altimeter,float Gyroscope_X_axis,float Gyroscope_Y_axis,float Gyroscope_Z_axis,float Gyroscope_W_axis,float GPS_X_axis,float GPS_Y_axis,float total_energy,float energy_loss_rate,float efficiency,float thermometer,float barometer,float wind_direction_X_axis,float wind_direction_Y_axis,float wind_direction_Z_axis,const float *repr_offset_q)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char buf[MAVLINK_MSG_ID_ATTITUDE_QUATERNION_LEN];
    _mav_put_float(buf, 0, Airspeed);
    _mav_put_float(buf, 4, Altimeter);
    _mav_put_float(buf, 8, Gyroscope_X_axis);
    _mav_put_float(buf, 12, Gyroscope_Y_axis);
    _mav_put_float(buf, 16, Gyroscope_Z_axis);
    _mav_put_float(buf, 20, Gyroscope_W_axis);
    _mav_put_float(buf, 24, GPS_X_axis);
    _mav_put_float(buf, 28, GPS_Y_axis);
    _mav_put_float(buf, 32, total_energy);
    _mav_put_float(buf, 36, energy_loss_rate);
    _mav_put_float(buf, 40, efficiency);
    _mav_put_float(buf, 44, thermometer);
    _mav_put_float(buf, 48, barometer);
    _mav_put_float(buf, 52, wind_direction_X_axis);
    _mav_put_float(buf, 56, wind_direction_Y_axis);
    _mav_put_float(buf, 60, wind_direction_Z_axis);
    _mav_put_float_array(buf, 64, repr_offset_q, 4);
        memcpy(_MAV_PAYLOAD_NON_CONST(msg), buf, MAVLINK_MSG_ID_ATTITUDE_QUATERNION_LEN);
#else
    mavlink_attitude_quaternion_t packet;
    packet.Airspeed = Airspeed;
    packet.Altimeter = Altimeter;
    packet.Gyroscope_X_axis = Gyroscope_X_axis;
    packet.Gyroscope_Y_axis = Gyroscope_Y_axis;
    packet.Gyroscope_Z_axis = Gyroscope_Z_axis;
    packet.Gyroscope_W_axis = Gyroscope_W_axis;
    packet.GPS_X_axis = GPS_X_axis;
    packet.GPS_Y_axis = GPS_Y_axis;
    packet.total_energy = total_energy;
    packet.energy_loss_rate = energy_loss_rate;
    packet.efficiency = efficiency;
    packet.thermometer = thermometer;
    packet.barometer = barometer;
    packet.wind_direction_X_axis = wind_direction_X_axis;
    packet.wind_direction_Y_axis = wind_direction_Y_axis;
    packet.wind_direction_Z_axis = wind_direction_Z_axis;
    mav_array_memcpy(packet.repr_offset_q, repr_offset_q, sizeof(float)*4);
        memcpy(_MAV_PAYLOAD_NON_CONST(msg), &packet, MAVLINK_MSG_ID_ATTITUDE_QUATERNION_LEN);
#endif

    msg->msgid = MAVLINK_MSG_ID_ATTITUDE_QUATERNION;
    return mavlink_finalize_message_chan(msg, system_id, component_id, chan, MAVLINK_MSG_ID_ATTITUDE_QUATERNION_MIN_LEN, MAVLINK_MSG_ID_ATTITUDE_QUATERNION_LEN, MAVLINK_MSG_ID_ATTITUDE_QUATERNION_CRC);
}

/**
 * @brief Encode a attitude_quaternion struct
 *
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param msg The MAVLink message to compress the data into
 * @param attitude_quaternion C-struct to read the message contents from
 */
static inline uint16_t mavlink_msg_attitude_quaternion_encode(uint8_t system_id, uint8_t component_id, mavlink_message_t* msg, const mavlink_attitude_quaternion_t* attitude_quaternion)
{
    return mavlink_msg_attitude_quaternion_pack(system_id, component_id, msg, attitude_quaternion->Airspeed, attitude_quaternion->Altimeter, attitude_quaternion->Gyroscope_X_axis, attitude_quaternion->Gyroscope_Y_axis, attitude_quaternion->Gyroscope_Z_axis, attitude_quaternion->Gyroscope_W_axis, attitude_quaternion->GPS_X_axis, attitude_quaternion->GPS_Y_axis, attitude_quaternion->total_energy, attitude_quaternion->energy_loss_rate, attitude_quaternion->efficiency, attitude_quaternion->thermometer, attitude_quaternion->barometer, attitude_quaternion->wind_direction_X_axis, attitude_quaternion->wind_direction_Y_axis, attitude_quaternion->wind_direction_Z_axis, attitude_quaternion->repr_offset_q);
}

/**
 * @brief Encode a attitude_quaternion struct on a channel
 *
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param chan The MAVLink channel this message will be sent over
 * @param msg The MAVLink message to compress the data into
 * @param attitude_quaternion C-struct to read the message contents from
 */
static inline uint16_t mavlink_msg_attitude_quaternion_encode_chan(uint8_t system_id, uint8_t component_id, uint8_t chan, mavlink_message_t* msg, const mavlink_attitude_quaternion_t* attitude_quaternion)
{
    return mavlink_msg_attitude_quaternion_pack_chan(system_id, component_id, chan, msg, attitude_quaternion->Airspeed, attitude_quaternion->Altimeter, attitude_quaternion->Gyroscope_X_axis, attitude_quaternion->Gyroscope_Y_axis, attitude_quaternion->Gyroscope_Z_axis, attitude_quaternion->Gyroscope_W_axis, attitude_quaternion->GPS_X_axis, attitude_quaternion->GPS_Y_axis, attitude_quaternion->total_energy, attitude_quaternion->energy_loss_rate, attitude_quaternion->efficiency, attitude_quaternion->thermometer, attitude_quaternion->barometer, attitude_quaternion->wind_direction_X_axis, attitude_quaternion->wind_direction_Y_axis, attitude_quaternion->wind_direction_Z_axis, attitude_quaternion->repr_offset_q);
}

/**
 * @brief Encode a attitude_quaternion struct with provided status structure
 *
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param status MAVLink status structure
 * @param msg The MAVLink message to compress the data into
 * @param attitude_quaternion C-struct to read the message contents from
 */
static inline uint16_t mavlink_msg_attitude_quaternion_encode_status(uint8_t system_id, uint8_t component_id, mavlink_status_t* _status, mavlink_message_t* msg, const mavlink_attitude_quaternion_t* attitude_quaternion)
{
    return mavlink_msg_attitude_quaternion_pack_status(system_id, component_id, _status, msg,  attitude_quaternion->Airspeed, attitude_quaternion->Altimeter, attitude_quaternion->Gyroscope_X_axis, attitude_quaternion->Gyroscope_Y_axis, attitude_quaternion->Gyroscope_Z_axis, attitude_quaternion->Gyroscope_W_axis, attitude_quaternion->GPS_X_axis, attitude_quaternion->GPS_Y_axis, attitude_quaternion->total_energy, attitude_quaternion->energy_loss_rate, attitude_quaternion->efficiency, attitude_quaternion->thermometer, attitude_quaternion->barometer, attitude_quaternion->wind_direction_X_axis, attitude_quaternion->wind_direction_Y_axis, attitude_quaternion->wind_direction_Z_axis, attitude_quaternion->repr_offset_q);
}

/**
 * @brief Send a attitude_quaternion message
 * @param chan MAVLink channel to send the message
 *
 * @param Airspeed [rad/s]  Airspeed
 * @param Altimeter [rad/s] Altitude
 * @param Gyroscope_X_axis [rad/s] Gyroscope in the X dimension
 * @param Gyroscope_Y_axis [rad/s] Gyroscope in the Y dimension
 * @param Gyroscope_Z_axis [rad/s] Gyroscope in the Z dimension
 * @param Gyroscope_W_axis [rad/s] Scalar of the Gyroscope
 * @param GPS_X_axis [rad/s] GPS in the X axis
 * @param GPS_Y_axis [rad/s] GPS in the Y axis
 * @param total_energy [rad/s] Total energy of Kinetic and Potential
        Energy added together
 * @param energy_loss_rate [rad/s] Loss of total energy
 * @param efficiency [rad/s] Efficiency of system
 * @param thermometer [rad/s] Measures temperature
 * @param barometer [rad/s] Measures pressure
 * @param wind_direction_X_axis [rad/s] Direction of wind in X axis
 * @param wind_direction_Y_axis [rad/s] Direction of wind in Y axis
 * @param wind_direction_Z_axis [rad/s] Direction of wind in Z axis
 * @param repr_offset_q  Rotation offset by which the attitude quaternion
        and angular speed vector should be rotated for user display (quaternion with [w, x, y, z]
        order, zero-rotation is [1, 0, 0, 0], send [0, 0, 0, 0] if field not supported). This field
        is intended for systems in which the reference attitude may change during flight. For
        example, tailsitters VTOLs rotate their reference attitude by 90 degrees between hover mode
        and fixed wing mode, thus repr_offset_q is equal to [1, 0, 0, 0] in hover mode and equal to
        [0.7071, 0, 0.7071, 0] in fixed wing mode.
 */
#ifdef MAVLINK_USE_CONVENIENCE_FUNCTIONS

static inline void mavlink_msg_attitude_quaternion_send(mavlink_channel_t chan, float Airspeed, float Altimeter, float Gyroscope_X_axis, float Gyroscope_Y_axis, float Gyroscope_Z_axis, float Gyroscope_W_axis, float GPS_X_axis, float GPS_Y_axis, float total_energy, float energy_loss_rate, float efficiency, float thermometer, float barometer, float wind_direction_X_axis, float wind_direction_Y_axis, float wind_direction_Z_axis, const float *repr_offset_q)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char buf[MAVLINK_MSG_ID_ATTITUDE_QUATERNION_LEN];
    _mav_put_float(buf, 0, Airspeed);
    _mav_put_float(buf, 4, Altimeter);
    _mav_put_float(buf, 8, Gyroscope_X_axis);
    _mav_put_float(buf, 12, Gyroscope_Y_axis);
    _mav_put_float(buf, 16, Gyroscope_Z_axis);
    _mav_put_float(buf, 20, Gyroscope_W_axis);
    _mav_put_float(buf, 24, GPS_X_axis);
    _mav_put_float(buf, 28, GPS_Y_axis);
    _mav_put_float(buf, 32, total_energy);
    _mav_put_float(buf, 36, energy_loss_rate);
    _mav_put_float(buf, 40, efficiency);
    _mav_put_float(buf, 44, thermometer);
    _mav_put_float(buf, 48, barometer);
    _mav_put_float(buf, 52, wind_direction_X_axis);
    _mav_put_float(buf, 56, wind_direction_Y_axis);
    _mav_put_float(buf, 60, wind_direction_Z_axis);
    _mav_put_float_array(buf, 64, repr_offset_q, 4);
    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_ATTITUDE_QUATERNION, buf, MAVLINK_MSG_ID_ATTITUDE_QUATERNION_MIN_LEN, MAVLINK_MSG_ID_ATTITUDE_QUATERNION_LEN, MAVLINK_MSG_ID_ATTITUDE_QUATERNION_CRC);
#else
    mavlink_attitude_quaternion_t packet;
    packet.Airspeed = Airspeed;
    packet.Altimeter = Altimeter;
    packet.Gyroscope_X_axis = Gyroscope_X_axis;
    packet.Gyroscope_Y_axis = Gyroscope_Y_axis;
    packet.Gyroscope_Z_axis = Gyroscope_Z_axis;
    packet.Gyroscope_W_axis = Gyroscope_W_axis;
    packet.GPS_X_axis = GPS_X_axis;
    packet.GPS_Y_axis = GPS_Y_axis;
    packet.total_energy = total_energy;
    packet.energy_loss_rate = energy_loss_rate;
    packet.efficiency = efficiency;
    packet.thermometer = thermometer;
    packet.barometer = barometer;
    packet.wind_direction_X_axis = wind_direction_X_axis;
    packet.wind_direction_Y_axis = wind_direction_Y_axis;
    packet.wind_direction_Z_axis = wind_direction_Z_axis;
    mav_array_memcpy(packet.repr_offset_q, repr_offset_q, sizeof(float)*4);
    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_ATTITUDE_QUATERNION, (const char *)&packet, MAVLINK_MSG_ID_ATTITUDE_QUATERNION_MIN_LEN, MAVLINK_MSG_ID_ATTITUDE_QUATERNION_LEN, MAVLINK_MSG_ID_ATTITUDE_QUATERNION_CRC);
#endif
}

/**
 * @brief Send a attitude_quaternion message
 * @param chan MAVLink channel to send the message
 * @param struct The MAVLink struct to serialize
 */
static inline void mavlink_msg_attitude_quaternion_send_struct(mavlink_channel_t chan, const mavlink_attitude_quaternion_t* attitude_quaternion)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    mavlink_msg_attitude_quaternion_send(chan, attitude_quaternion->Airspeed, attitude_quaternion->Altimeter, attitude_quaternion->Gyroscope_X_axis, attitude_quaternion->Gyroscope_Y_axis, attitude_quaternion->Gyroscope_Z_axis, attitude_quaternion->Gyroscope_W_axis, attitude_quaternion->GPS_X_axis, attitude_quaternion->GPS_Y_axis, attitude_quaternion->total_energy, attitude_quaternion->energy_loss_rate, attitude_quaternion->efficiency, attitude_quaternion->thermometer, attitude_quaternion->barometer, attitude_quaternion->wind_direction_X_axis, attitude_quaternion->wind_direction_Y_axis, attitude_quaternion->wind_direction_Z_axis, attitude_quaternion->repr_offset_q);
#else
    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_ATTITUDE_QUATERNION, (const char *)attitude_quaternion, MAVLINK_MSG_ID_ATTITUDE_QUATERNION_MIN_LEN, MAVLINK_MSG_ID_ATTITUDE_QUATERNION_LEN, MAVLINK_MSG_ID_ATTITUDE_QUATERNION_CRC);
#endif
}

#if MAVLINK_MSG_ID_ATTITUDE_QUATERNION_LEN <= MAVLINK_MAX_PAYLOAD_LEN
/*
  This variant of _send() can be used to save stack space by re-using
  memory from the receive buffer.  The caller provides a
  mavlink_message_t which is the size of a full mavlink message. This
  is usually the receive buffer for the channel, and allows a reply to an
  incoming message with minimum stack space usage.
 */
static inline void mavlink_msg_attitude_quaternion_send_buf(mavlink_message_t *msgbuf, mavlink_channel_t chan,  float Airspeed, float Altimeter, float Gyroscope_X_axis, float Gyroscope_Y_axis, float Gyroscope_Z_axis, float Gyroscope_W_axis, float GPS_X_axis, float GPS_Y_axis, float total_energy, float energy_loss_rate, float efficiency, float thermometer, float barometer, float wind_direction_X_axis, float wind_direction_Y_axis, float wind_direction_Z_axis, const float *repr_offset_q)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char *buf = (char *)msgbuf;
    _mav_put_float(buf, 0, Airspeed);
    _mav_put_float(buf, 4, Altimeter);
    _mav_put_float(buf, 8, Gyroscope_X_axis);
    _mav_put_float(buf, 12, Gyroscope_Y_axis);
    _mav_put_float(buf, 16, Gyroscope_Z_axis);
    _mav_put_float(buf, 20, Gyroscope_W_axis);
    _mav_put_float(buf, 24, GPS_X_axis);
    _mav_put_float(buf, 28, GPS_Y_axis);
    _mav_put_float(buf, 32, total_energy);
    _mav_put_float(buf, 36, energy_loss_rate);
    _mav_put_float(buf, 40, efficiency);
    _mav_put_float(buf, 44, thermometer);
    _mav_put_float(buf, 48, barometer);
    _mav_put_float(buf, 52, wind_direction_X_axis);
    _mav_put_float(buf, 56, wind_direction_Y_axis);
    _mav_put_float(buf, 60, wind_direction_Z_axis);
    _mav_put_float_array(buf, 64, repr_offset_q, 4);
    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_ATTITUDE_QUATERNION, buf, MAVLINK_MSG_ID_ATTITUDE_QUATERNION_MIN_LEN, MAVLINK_MSG_ID_ATTITUDE_QUATERNION_LEN, MAVLINK_MSG_ID_ATTITUDE_QUATERNION_CRC);
#else
    mavlink_attitude_quaternion_t *packet = (mavlink_attitude_quaternion_t *)msgbuf;
    packet->Airspeed = Airspeed;
    packet->Altimeter = Altimeter;
    packet->Gyroscope_X_axis = Gyroscope_X_axis;
    packet->Gyroscope_Y_axis = Gyroscope_Y_axis;
    packet->Gyroscope_Z_axis = Gyroscope_Z_axis;
    packet->Gyroscope_W_axis = Gyroscope_W_axis;
    packet->GPS_X_axis = GPS_X_axis;
    packet->GPS_Y_axis = GPS_Y_axis;
    packet->total_energy = total_energy;
    packet->energy_loss_rate = energy_loss_rate;
    packet->efficiency = efficiency;
    packet->thermometer = thermometer;
    packet->barometer = barometer;
    packet->wind_direction_X_axis = wind_direction_X_axis;
    packet->wind_direction_Y_axis = wind_direction_Y_axis;
    packet->wind_direction_Z_axis = wind_direction_Z_axis;
    mav_array_memcpy(packet->repr_offset_q, repr_offset_q, sizeof(float)*4);
    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_ATTITUDE_QUATERNION, (const char *)packet, MAVLINK_MSG_ID_ATTITUDE_QUATERNION_MIN_LEN, MAVLINK_MSG_ID_ATTITUDE_QUATERNION_LEN, MAVLINK_MSG_ID_ATTITUDE_QUATERNION_CRC);
#endif
}
#endif

#endif

// MESSAGE ATTITUDE_QUATERNION UNPACKING


/**
 * @brief Get field Airspeed from attitude_quaternion message
 *
 * @return [rad/s]  Airspeed
 */
static inline float mavlink_msg_attitude_quaternion_get_Airspeed(const mavlink_message_t* msg)
{
    return _MAV_RETURN_float(msg,  0);
}

/**
 * @brief Get field Altimeter from attitude_quaternion message
 *
 * @return [rad/s] Altitude
 */
static inline float mavlink_msg_attitude_quaternion_get_Altimeter(const mavlink_message_t* msg)
{
    return _MAV_RETURN_float(msg,  4);
}

/**
 * @brief Get field Gyroscope_X_axis from attitude_quaternion message
 *
 * @return [rad/s] Gyroscope in the X dimension
 */
static inline float mavlink_msg_attitude_quaternion_get_Gyroscope_X_axis(const mavlink_message_t* msg)
{
    return _MAV_RETURN_float(msg,  8);
}

/**
 * @brief Get field Gyroscope_Y_axis from attitude_quaternion message
 *
 * @return [rad/s] Gyroscope in the Y dimension
 */
static inline float mavlink_msg_attitude_quaternion_get_Gyroscope_Y_axis(const mavlink_message_t* msg)
{
    return _MAV_RETURN_float(msg,  12);
}

/**
 * @brief Get field Gyroscope_Z_axis from attitude_quaternion message
 *
 * @return [rad/s] Gyroscope in the Z dimension
 */
static inline float mavlink_msg_attitude_quaternion_get_Gyroscope_Z_axis(const mavlink_message_t* msg)
{
    return _MAV_RETURN_float(msg,  16);
}

/**
 * @brief Get field Gyroscope_W_axis from attitude_quaternion message
 *
 * @return [rad/s] Scalar of the Gyroscope
 */
static inline float mavlink_msg_attitude_quaternion_get_Gyroscope_W_axis(const mavlink_message_t* msg)
{
    return _MAV_RETURN_float(msg,  20);
}

/**
 * @brief Get field GPS_X_axis from attitude_quaternion message
 *
 * @return [rad/s] GPS in the X axis
 */
static inline float mavlink_msg_attitude_quaternion_get_GPS_X_axis(const mavlink_message_t* msg)
{
    return _MAV_RETURN_float(msg,  24);
}

/**
 * @brief Get field GPS_Y_axis from attitude_quaternion message
 *
 * @return [rad/s] GPS in the Y axis
 */
static inline float mavlink_msg_attitude_quaternion_get_GPS_Y_axis(const mavlink_message_t* msg)
{
    return _MAV_RETURN_float(msg,  28);
}

/**
 * @brief Get field total_energy from attitude_quaternion message
 *
 * @return [rad/s] Total energy of Kinetic and Potential
        Energy added together
 */
static inline float mavlink_msg_attitude_quaternion_get_total_energy(const mavlink_message_t* msg)
{
    return _MAV_RETURN_float(msg,  32);
}

/**
 * @brief Get field energy_loss_rate from attitude_quaternion message
 *
 * @return [rad/s] Loss of total energy
 */
static inline float mavlink_msg_attitude_quaternion_get_energy_loss_rate(const mavlink_message_t* msg)
{
    return _MAV_RETURN_float(msg,  36);
}

/**
 * @brief Get field efficiency from attitude_quaternion message
 *
 * @return [rad/s] Efficiency of system
 */
static inline float mavlink_msg_attitude_quaternion_get_efficiency(const mavlink_message_t* msg)
{
    return _MAV_RETURN_float(msg,  40);
}

/**
 * @brief Get field thermometer from attitude_quaternion message
 *
 * @return [rad/s] Measures temperature
 */
static inline float mavlink_msg_attitude_quaternion_get_thermometer(const mavlink_message_t* msg)
{
    return _MAV_RETURN_float(msg,  44);
}

/**
 * @brief Get field barometer from attitude_quaternion message
 *
 * @return [rad/s] Measures pressure
 */
static inline float mavlink_msg_attitude_quaternion_get_barometer(const mavlink_message_t* msg)
{
    return _MAV_RETURN_float(msg,  48);
}

/**
 * @brief Get field wind_direction_X_axis from attitude_quaternion message
 *
 * @return [rad/s] Direction of wind in X axis
 */
static inline float mavlink_msg_attitude_quaternion_get_wind_direction_X_axis(const mavlink_message_t* msg)
{
    return _MAV_RETURN_float(msg,  52);
}

/**
 * @brief Get field wind_direction_Y_axis from attitude_quaternion message
 *
 * @return [rad/s] Direction of wind in Y axis
 */
static inline float mavlink_msg_attitude_quaternion_get_wind_direction_Y_axis(const mavlink_message_t* msg)
{
    return _MAV_RETURN_float(msg,  56);
}

/**
 * @brief Get field wind_direction_Z_axis from attitude_quaternion message
 *
 * @return [rad/s] Direction of wind in Z axis
 */
static inline float mavlink_msg_attitude_quaternion_get_wind_direction_Z_axis(const mavlink_message_t* msg)
{
    return _MAV_RETURN_float(msg,  60);
}

/**
 * @brief Get field repr_offset_q from attitude_quaternion message
 *
 * @return  Rotation offset by which the attitude quaternion
        and angular speed vector should be rotated for user display (quaternion with [w, x, y, z]
        order, zero-rotation is [1, 0, 0, 0], send [0, 0, 0, 0] if field not supported). This field
        is intended for systems in which the reference attitude may change during flight. For
        example, tailsitters VTOLs rotate their reference attitude by 90 degrees between hover mode
        and fixed wing mode, thus repr_offset_q is equal to [1, 0, 0, 0] in hover mode and equal to
        [0.7071, 0, 0.7071, 0] in fixed wing mode.
 */
static inline uint16_t mavlink_msg_attitude_quaternion_get_repr_offset_q(const mavlink_message_t* msg, float *repr_offset_q)
{
    return _MAV_RETURN_float_array(msg, repr_offset_q, 4,  64);
}

/**
 * @brief Decode a attitude_quaternion message into a struct
 *
 * @param msg The message to decode
 * @param attitude_quaternion C-struct to decode the message contents into
 */
static inline void mavlink_msg_attitude_quaternion_decode(const mavlink_message_t* msg, mavlink_attitude_quaternion_t* attitude_quaternion)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    attitude_quaternion->Airspeed = mavlink_msg_attitude_quaternion_get_Airspeed(msg);
    attitude_quaternion->Altimeter = mavlink_msg_attitude_quaternion_get_Altimeter(msg);
    attitude_quaternion->Gyroscope_X_axis = mavlink_msg_attitude_quaternion_get_Gyroscope_X_axis(msg);
    attitude_quaternion->Gyroscope_Y_axis = mavlink_msg_attitude_quaternion_get_Gyroscope_Y_axis(msg);
    attitude_quaternion->Gyroscope_Z_axis = mavlink_msg_attitude_quaternion_get_Gyroscope_Z_axis(msg);
    attitude_quaternion->Gyroscope_W_axis = mavlink_msg_attitude_quaternion_get_Gyroscope_W_axis(msg);
    attitude_quaternion->GPS_X_axis = mavlink_msg_attitude_quaternion_get_GPS_X_axis(msg);
    attitude_quaternion->GPS_Y_axis = mavlink_msg_attitude_quaternion_get_GPS_Y_axis(msg);
    attitude_quaternion->total_energy = mavlink_msg_attitude_quaternion_get_total_energy(msg);
    attitude_quaternion->energy_loss_rate = mavlink_msg_attitude_quaternion_get_energy_loss_rate(msg);
    attitude_quaternion->efficiency = mavlink_msg_attitude_quaternion_get_efficiency(msg);
    attitude_quaternion->thermometer = mavlink_msg_attitude_quaternion_get_thermometer(msg);
    attitude_quaternion->barometer = mavlink_msg_attitude_quaternion_get_barometer(msg);
    attitude_quaternion->wind_direction_X_axis = mavlink_msg_attitude_quaternion_get_wind_direction_X_axis(msg);
    attitude_quaternion->wind_direction_Y_axis = mavlink_msg_attitude_quaternion_get_wind_direction_Y_axis(msg);
    attitude_quaternion->wind_direction_Z_axis = mavlink_msg_attitude_quaternion_get_wind_direction_Z_axis(msg);
    mavlink_msg_attitude_quaternion_get_repr_offset_q(msg, attitude_quaternion->repr_offset_q);
#else
        uint8_t len = msg->len < MAVLINK_MSG_ID_ATTITUDE_QUATERNION_LEN? msg->len : MAVLINK_MSG_ID_ATTITUDE_QUATERNION_LEN;
        memset(attitude_quaternion, 0, MAVLINK_MSG_ID_ATTITUDE_QUATERNION_LEN);
    memcpy(attitude_quaternion, _MAV_PAYLOAD(msg), len);
#endif
}
