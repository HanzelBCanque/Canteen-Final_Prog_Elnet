namespace CANTEEN_SYSTEM.Contracts;

public record ProductDto(int Id, string Name, decimal Price, int Stock, int CategoryId, string CategoryName, string ImageUrl);
