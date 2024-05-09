namespace DeliverySystem.Bits.Enums;

public enum JobState
{
    Queued = 0,
    Connecting = 1,
    Transferring = 2,
    Suspended = 3,
    Error = 4,
    TransientError = 5,
    Transferred = 6,
    Acknowledged = 7,
    Canceled = 8,
}
