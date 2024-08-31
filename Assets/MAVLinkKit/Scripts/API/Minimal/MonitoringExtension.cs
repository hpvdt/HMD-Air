using System;
using System.Collections.Generic;
using MAVLinkKit.Scripts.Util;
using Unity.VisualScripting;

namespace MAVLinkKit.Scripts.API.Minimal
{
    // public class Monitor<T>
    // {
    // }

    public static class MonitoringExtension
    {
        public static MAVConnection.ReadAPI<T> ReadAndMonitor<T>(this MAVConnection connection) where T : struct
        {
            // this will return an empty builder that respond to heartbeat and request target to send all data
            // will fail if heartbeat is not received within 2 seconds

            var api = connection.Read<T>()
                .On<MAVLink.mavlink_heartbeat_t>()
                .SelectMany(
                    ctx =>
                    {
                        var sender = ctx.Msg.Sender;

                        // var heartbeatBack = ctx.Msg.Data;
                        var heartbeatBack =
                            new MAVLink.mavlink_heartbeat_t() // this should be sent regardless of received heartbeat
                            {
                                custom_mode = 0, // not sure how to use this
                                mavlink_version = ctx.Msg.Data.mavlink_version,
                                type = (byte)MAVLink.MAV_TYPE.GCS,
                                autopilot = (byte)MAVLink.MAV_AUTOPILOT.INVALID,
                                base_mode = 0
                            };

                        // TODO: may be too frequent, should only send once
                        var requestAll = new MAVLink.mavlink_request_data_stream_t()
                        {
                            req_message_rate = 2,
                            req_stream_id = (byte)MAVLink.MAV_DATA_STREAM.ALL,
                            start_stop = 1,
                            target_component = sender.ComponentID,
                            target_system = sender.SystemID
                        };

                        connection.WriteData(heartbeatBack);
                        connection.WriteData(requestAll);

                        return new List<T>();
                    }
                );

            var reader = api.Build();

            var retry = Retry.Of(12, TimeSpan.FromSeconds(0.2)).FixedInterval;

            retry.Run(
                (_, tt) =>
                {
                    reader.Drain();

                    if (api.Outer.Stats.Counter.Get<MAVLink.mavlink_heartbeat_t>().Value <= 0)
                    {
                        throw new InvalidConnectionException($"No heartbeat received after {tt}");
                    }
                }
            );

            return api;
        }
    }
}