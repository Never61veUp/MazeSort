namespace MazeSort;

public record State(string Hallway, string[] Rooms)
{
    public virtual bool Equals(State? obj)
    {
        if (obj is not State other)
            return false;

        return Hallway == other.Hallway &&
               Rooms.SequenceEqual(other.Rooms);
    }

    public override int GetHashCode()
    {
        var hash = Hallway.GetHashCode();
        
        foreach (var room in Rooms) 
            hash = HashCode.Combine(hash, room);
        
        return hash;
    }
}