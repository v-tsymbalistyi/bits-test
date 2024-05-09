using DeliverySystem.Bits.Enums;

namespace DeliverySystem.Bits.Base;

public class BitsCredentials
{
    public BitsCredentials(AuthenticationScheme authenticationScheme, AuthenticationTarget authenticationTarget, string userName, string password)
    {
        AuthenticationScheme = authenticationScheme;
        AuthenticationTarget = authenticationTarget;
        UserName = userName;
        Password = password;
    }

    public AuthenticationScheme AuthenticationScheme { get; set; }

    public AuthenticationTarget AuthenticationTarget { get; set; }

    public string UserName { get; set; }

    //TODO: secure this
    public string Password { get; set; }
}
