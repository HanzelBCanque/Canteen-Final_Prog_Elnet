namespace CANTEEN_SYSTEM.Contracts;

public record OrderItemDto(int ProductId, string ProductName, int Quantity, decimal Price);
