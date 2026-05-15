namespace CANTEEN_SYSTEM.Contracts;

public class UpdateOrderStatusRequest
{
    public int ActorEmployeeId { get; set; }
    public string Status { get; set; } = string.Empty;
}
