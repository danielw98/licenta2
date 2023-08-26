public class CellEntity
{
    public CellEntity Parent { get; set; }
    public int Id { get; set; }
    public bool IsEntry { get; set; }
    public bool IsExit { get; set; }
    public bool HasBuilding { get; set; }
    public int X { get; }
    public int Y { get; }
    
    public int F { get; set; }
    public int H { get; set; }
    public int G { get; set; }

    public CellEntity(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString()
    {
        return "X: " + X + ", Y: " + Y + ", IsEntry: " + IsEntry + ", IsExit: " + IsExit + ", HasBuilding: " + HasBuilding;
    }
}
