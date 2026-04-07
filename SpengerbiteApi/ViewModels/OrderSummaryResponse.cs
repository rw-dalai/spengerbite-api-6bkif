namespace SpengerbiteApi.ViewModels;

public record OrderSummaryResponse(
    int OrderId,
    string RestaurantName,
    string Status,
    int ItemCount,
    decimal Total
);
