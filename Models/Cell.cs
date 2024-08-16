namespace GameOfLifeApi.Models;

public class Cell
{
    public int Id { get; set; }
    public int Iteration { get; set; }
    public string? Uuid { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public bool IsAlive { get; set; }
}