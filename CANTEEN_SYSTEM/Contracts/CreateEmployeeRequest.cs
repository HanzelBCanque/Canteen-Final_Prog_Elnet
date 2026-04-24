namespace CANTEEN_SYSTEM.Contracts;

public class CreateEmployeeRequest
{
    public int ActorEmployeeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Pin { get; set; } = string.Empty;
    public string Role { get; set; } = "cashier";
}
