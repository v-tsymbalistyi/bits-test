namespace DeliverySystem.Bits.Enums;

[Flags()]
public enum NotificationFlags
{
    /// <summary>
    /// All of the files in the job have been transferred.
    /// </summary>
    JobTransferred = 1,
    /// <summary>
    /// An error has occurred
    /// </summary>
    JobErrorOccured = 2,
    /// <summary>
    /// Event notification is disabled. BITS ignores the other flags.
    /// </summary>
    NotificationDisabled = 4,
    /// <summary>
    /// The job has been modified. For example, a property value changed, the state of the job changed, or progress is made transferring the files.
    /// </summary>
    JobModified = 8,
}
