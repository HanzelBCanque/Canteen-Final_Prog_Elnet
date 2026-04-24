namespace CANTEEN_SYSTEM.Contracts;

public record OrderDto(
    int Id,
    string OrderNumber,
    decimal TotalAmount,
    string PaymentMethod,
    string Status,
    DateTime CreatedAt,
    string? ReferenceNumber,
    decimal? AmountReceived,
    decimal? Change,
    IReadOnlyList<OrderItemDto> Items);
