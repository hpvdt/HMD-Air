/** @file
 *    @brief MAVLink comm protocol testsuite generated from falcon_sensors.xml
 *    @see https://mavlink.io/en/
 */
#pragma once
#ifndef FALCON_SENSORS_TESTSUITE_H
#define FALCON_SENSORS_TESTSUITE_H

#ifdef __cplusplus
extern "C" {
#endif

#ifndef MAVLINK_TEST_ALL
#define MAVLINK_TEST_ALL

static void mavlink_test_falcon_sensors(uint8_t, uint8_t, mavlink_message_t *last_msg);

static void mavlink_test_all(uint8_t system_id, uint8_t component_id, mavlink_message_t *last_msg)
{

    mavlink_test_falcon_sensors(system_id, component_id, last_msg);
}
#endif




static void mavlink_test_attitude_quaternion(uint8_t system_id, uint8_t component_id, mavlink_message_t *last_msg)
{
#ifdef MAVLINK_STATUS_FLAG_OUT_MAVLINK1
    mavlink_status_t *status = mavlink_get_channel_status(MAVLINK_COMM_0);
        if ((status->flags & MAVLINK_STATUS_FLAG_OUT_MAVLINK1) && MAVLINK_MSG_ID_ATTITUDE_QUATERNION >= 256) {
            return;
        }
#endif
    mavlink_message_t msg;
        uint8_t buffer[MAVLINK_MAX_PACKET_LEN];
        uint16_t i;
    mavlink_attitude_quaternion_t packet_in = {
        17.0,45.0,73.0,101.0,129.0,157.0,185.0,213.0,241.0,269.0,297.0,325.0,353.0,381.0,409.0,437.0,{ 465.0, 466.0, 467.0, 468.0 }
    };
    mavlink_attitude_quaternion_t packet1, packet2;
        memset(&packet1, 0, sizeof(packet1));
        packet1.Airspeed = packet_in.Airspeed;
        packet1.Altimeter = packet_in.Altimeter;
        packet1.Gyroscope_X_axis = packet_in.Gyroscope_X_axis;
        packet1.Gyroscope_Y_axis = packet_in.Gyroscope_Y_axis;
        packet1.Gyroscope_Z_axis = packet_in.Gyroscope_Z_axis;
        packet1.Gyroscope_W_axis = packet_in.Gyroscope_W_axis;
        packet1.GPS_X_axis = packet_in.GPS_X_axis;
        packet1.GPS_Y_axis = packet_in.GPS_Y_axis;
        packet1.total_energy = packet_in.total_energy;
        packet1.energy_loss_rate = packet_in.energy_loss_rate;
        packet1.efficiency = packet_in.efficiency;
        packet1.thermometer = packet_in.thermometer;
        packet1.barometer = packet_in.barometer;
        packet1.wind_direction_X_axis = packet_in.wind_direction_X_axis;
        packet1.wind_direction_Y_axis = packet_in.wind_direction_Y_axis;
        packet1.wind_direction_Z_axis = packet_in.wind_direction_Z_axis;
        
        mav_array_memcpy(packet1.repr_offset_q, packet_in.repr_offset_q, sizeof(float)*4);
        
#ifdef MAVLINK_STATUS_FLAG_OUT_MAVLINK1
        if (status->flags & MAVLINK_STATUS_FLAG_OUT_MAVLINK1) {
           // cope with extensions
           memset(MAVLINK_MSG_ID_ATTITUDE_QUATERNION_MIN_LEN + (char *)&packet1, 0, sizeof(packet1)-MAVLINK_MSG_ID_ATTITUDE_QUATERNION_MIN_LEN);
        }
#endif
        memset(&packet2, 0, sizeof(packet2));
    mavlink_msg_attitude_quaternion_encode(system_id, component_id, &msg, &packet1);
    mavlink_msg_attitude_quaternion_decode(&msg, &packet2);
        MAVLINK_ASSERT(memcmp(&packet1, &packet2, sizeof(packet1)) == 0);

        memset(&packet2, 0, sizeof(packet2));
    mavlink_msg_attitude_quaternion_pack(system_id, component_id, &msg , packet1.Airspeed , packet1.Altimeter , packet1.Gyroscope_X_axis , packet1.Gyroscope_Y_axis , packet1.Gyroscope_Z_axis , packet1.Gyroscope_W_axis , packet1.GPS_X_axis , packet1.GPS_Y_axis , packet1.total_energy , packet1.energy_loss_rate , packet1.efficiency , packet1.thermometer , packet1.barometer , packet1.wind_direction_X_axis , packet1.wind_direction_Y_axis , packet1.wind_direction_Z_axis , packet1.repr_offset_q );
    mavlink_msg_attitude_quaternion_decode(&msg, &packet2);
        MAVLINK_ASSERT(memcmp(&packet1, &packet2, sizeof(packet1)) == 0);

        memset(&packet2, 0, sizeof(packet2));
    mavlink_msg_attitude_quaternion_pack_chan(system_id, component_id, MAVLINK_COMM_0, &msg , packet1.Airspeed , packet1.Altimeter , packet1.Gyroscope_X_axis , packet1.Gyroscope_Y_axis , packet1.Gyroscope_Z_axis , packet1.Gyroscope_W_axis , packet1.GPS_X_axis , packet1.GPS_Y_axis , packet1.total_energy , packet1.energy_loss_rate , packet1.efficiency , packet1.thermometer , packet1.barometer , packet1.wind_direction_X_axis , packet1.wind_direction_Y_axis , packet1.wind_direction_Z_axis , packet1.repr_offset_q );
    mavlink_msg_attitude_quaternion_decode(&msg, &packet2);
        MAVLINK_ASSERT(memcmp(&packet1, &packet2, sizeof(packet1)) == 0);

        memset(&packet2, 0, sizeof(packet2));
        mavlink_msg_to_send_buffer(buffer, &msg);
        for (i=0; i<mavlink_msg_get_send_buffer_length(&msg); i++) {
            comm_send_ch(MAVLINK_COMM_0, buffer[i]);
        }
    mavlink_msg_attitude_quaternion_decode(last_msg, &packet2);
        MAVLINK_ASSERT(memcmp(&packet1, &packet2, sizeof(packet1)) == 0);
        
        memset(&packet2, 0, sizeof(packet2));
    mavlink_msg_attitude_quaternion_send(MAVLINK_COMM_1 , packet1.Airspeed , packet1.Altimeter , packet1.Gyroscope_X_axis , packet1.Gyroscope_Y_axis , packet1.Gyroscope_Z_axis , packet1.Gyroscope_W_axis , packet1.GPS_X_axis , packet1.GPS_Y_axis , packet1.total_energy , packet1.energy_loss_rate , packet1.efficiency , packet1.thermometer , packet1.barometer , packet1.wind_direction_X_axis , packet1.wind_direction_Y_axis , packet1.wind_direction_Z_axis , packet1.repr_offset_q );
    mavlink_msg_attitude_quaternion_decode(last_msg, &packet2);
        MAVLINK_ASSERT(memcmp(&packet1, &packet2, sizeof(packet1)) == 0);

#ifdef MAVLINK_HAVE_GET_MESSAGE_INFO
    MAVLINK_ASSERT(mavlink_get_message_info_by_name("ATTITUDE_QUATERNION") != NULL);
    MAVLINK_ASSERT(mavlink_get_message_info_by_id(MAVLINK_MSG_ID_ATTITUDE_QUATERNION) != NULL);
#endif
}

static void mavlink_test_falcon_sensors(uint8_t system_id, uint8_t component_id, mavlink_message_t *last_msg)
{
    mavlink_test_attitude_quaternion(system_id, component_id, last_msg);
}

#ifdef __cplusplus
}
#endif // __cplusplus
#endif // FALCON_SENSORS_TESTSUITE_H
