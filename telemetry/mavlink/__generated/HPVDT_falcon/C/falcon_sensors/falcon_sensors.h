/** @file
 *  @brief MAVLink comm protocol generated from falcon_sensors.xml
 *  @see http://mavlink.org
 */
#pragma once
#ifndef MAVLINK_FALCON_SENSORS_H
#define MAVLINK_FALCON_SENSORS_H

#ifndef MAVLINK_H
    #error Wrong include order: MAVLINK_FALCON_SENSORS.H MUST NOT BE DIRECTLY USED. Include mavlink.h from the same directory instead or set ALL AND EVERY defines from MAVLINK.H manually accordingly, including the #define MAVLINK_H call.
#endif

#define MAVLINK_FALCON_SENSORS_XML_HASH 4477913971594980381

#ifdef __cplusplus
extern "C" {
#endif

// MESSAGE LENGTHS AND CRCS

#ifndef MAVLINK_MESSAGE_LENGTHS
#define MAVLINK_MESSAGE_LENGTHS {}
#endif

#ifndef MAVLINK_MESSAGE_CRCS
#define MAVLINK_MESSAGE_CRCS {{31, 219, 64, 80, 0, 0, 0}}
#endif

#include "../protocol.h"

#define MAVLINK_ENABLED_FALCON_SENSORS

// ENUM DEFINITIONS



// MAVLINK VERSION

#ifndef MAVLINK_VERSION
#define MAVLINK_VERSION 3
#endif

#if (MAVLINK_VERSION == 0)
#undef MAVLINK_VERSION
#define MAVLINK_VERSION 3
#endif

// MESSAGE DEFINITIONS
#include "./mavlink_msg_attitude_quaternion.h"

// base include



#if MAVLINK_FALCON_SENSORS_XML_HASH == MAVLINK_PRIMARY_XML_HASH
# define MAVLINK_MESSAGE_INFO {MAVLINK_MESSAGE_INFO_ATTITUDE_QUATERNION}
# define MAVLINK_MESSAGE_NAMES {{ "ATTITUDE_QUATERNION", 31 }}
# if MAVLINK_COMMAND_24BIT
#  include "../mavlink_get_info.h"
# endif
#endif

#ifdef __cplusplus
}
#endif // __cplusplus
#endif // MAVLINK_FALCON_SENSORS_H
