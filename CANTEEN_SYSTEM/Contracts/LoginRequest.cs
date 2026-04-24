namespace CANTEEN_SYSTEM.Contracts;

public class LoginRequest
{
    public string QrCode { get; set; } = string.Empty;
    public string Pin { get; set; } = string.Empty;
}
