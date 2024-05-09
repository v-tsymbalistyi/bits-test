namespace DeliverySystem.Bits.Enums;

public enum ErrorContext
{
    None = 0,
    UnknownError = 1,
    GeneralQueueManagerError = 2,
    QueueManagerNotificationError = 3,
    LocalFileError = 4,
    RemoteFileError = 5,
    GeneralTransportError = 6,
    RemoteApplicationError = 7,
}
