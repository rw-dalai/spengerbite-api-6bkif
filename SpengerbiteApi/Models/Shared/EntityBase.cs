namespace SpengerbiteApi.Models.Shared;

public abstract class EntityBase
{
    // --- Id ---

    // Set by EF Core, once the entity is saved to the database
    public int Id { get; set; }

    
    // --- Equals ---
    
    protected bool Equals(EntityBase other)
    {
        return Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((EntityBase)obj);
    }
    
    // --- Hash Code --
    
    public override int GetHashCode()
    {
        return Id;
    }
}
