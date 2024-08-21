import HPVDT_falcon.Python as falcon

if __name__ == '__main__':

    mavlink = falcon.MAVLink(object)

    msg = falcon.MAVLink_attitude_quaternion_message(
        Airspeed=1,
        Altimeter=2,
        Gyroscope_X_axis=1,
        Gyroscope_Y_axis=1,
        Gyroscope_Z_axis=1,
        Gyroscope_W_axis=1,
        GPS_X_axis=1,
        GPS_Y_axis=1,
        total_energy=1,
        energy_loss_rate=1,
        efficiency=1,
        thermometer=1,
        barometer=1,
        wind_direction_X_axis=1,
        wind_direction_Y_axis=1,
        wind_direction_Z_axis=1,
        repr_offset_q=(0, 0, 0, 0)
    )

    packed_msg = msg.pack(mavlink)

    # print(packed_msg)

    '''array1 = bytearray(str(packed_msg), 'utf-8')
    print('bytearray', type(array1))
    print('decoded', type(array1.decode()))'''

    # unpacked = MAVLink_message('Peepee', packed_msg)
    # print(unpacked)

    deserialize = mavlink.parse_buffer(packed_msg)
    deserialize = deserialize[0]

    print(deserialize)