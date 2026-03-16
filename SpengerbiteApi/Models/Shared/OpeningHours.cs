namespace SpengerbiteApi.Models.Shared;

public record OpeningHours
{
    public TimeOnly OpenTime { get; init; }

    public TimeOnly CloseTime { get; init; }

    public OpeningHours(TimeOnly openTime, TimeOnly closeTime)
    {
        // TODO: validation
        OpenTime = openTime;
        CloseTime = closeTime;
    }
}
