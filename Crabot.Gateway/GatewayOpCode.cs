namespace Crabot.Gateway
{
    public enum GatewayOpCode
    {
        // C <- S
        Dispatch = 0,

        // C <-> S
        Heartbeat = 1,

        // C -> S
        Identify = 2,

        StatusUpdate = 3,

        // C -> S 
        // Send after opening new connection
        Resume = 6,

        // C <- S    Client have to reconnect
        // After this event client should close old connection,
        // create new connection and send resume event (6) after it
        Reconnect = 7,

        RequestGuildMembers = 8,

        // C <- S   Session cannot be resumed, send new Identify event
        InvalidSession = 9,

        // C <- S
        Hello = 10,

        // C <- S
        HeartbeatAck = 11,

        GuildSync = 12
    }
}
