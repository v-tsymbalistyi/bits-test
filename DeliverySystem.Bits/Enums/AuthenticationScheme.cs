﻿namespace DeliverySystem.Bits.Enums;

/// <summary>
/// The AuthenticationScheme enumeration defines the constant values that specify the authentication scheme to use when a proxy or server requests user authentication.
/// </summary>
public enum AuthenticationScheme
{
    /// <summary>
    /// Basic is a scheme in which the user name and password are sent in clear-text to the server or proxy.
    /// </summary>
    Basic = 1,
    /// <summary>
    /// Digest is a challenge-response scheme that uses a server-specified data string for the challenge. 
    /// </summary>
    Digest,
    /// <summary>
    /// Windows NT LAN Manager (NTLM) is a challenge-response scheme that uses the credentials of the 
    /// user for authentication in a Windows network environment. 
    /// </summary>
    Ntlm,
    /// <summary>
    /// Simple and Protected Negotiation protocol (Snego) is a challenge-response scheme that negotiates 
    /// with the server or proxy to determine which scheme to use for authentication. Examples are the Kerberos protocol and NTLM
    /// </summary>
    Negotiate,
    /// <summary>
    /// Passport is a centralized authentication service provided by Microsoft that offers a single logon for member sites. 
    /// </summary>
    Passport
}
