namespace DeliverySystem.Bits.Enums;

/// <summary>
/// The AuthenticationTarget enumeration defines the constant values that specify whether the credentials are used for proxy or server user authentication requests.
/// </summary>
public enum AuthenticationTarget
{
    /// <summary>
    /// Use credentials for server requests.
    /// </summary>
    Server = 1,
    /// <summary>
    /// Use credentials for proxy requests. 
    /// </summary>
    Proxy = 2,
}
