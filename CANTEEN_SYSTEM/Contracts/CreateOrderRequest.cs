namespace CANTEEN_SYSTEM.Contracts;

public class CreateOrderRequest
{
    public string PaymentMethod { get; set; } = string.Empty;
    public string? ReferenceNumber { get; set; }
    public decimal? AmountReceived { get; set; }
    public List<CreateOrderItemRequest> Items { get; set; } = [];
}

public class CreateOrderItemRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
