using GameOfLifeApi.Models;
using GameOfLifeApi.Data;
using Microsoft.EntityFrameworkCore;

namespace GameOfLifeApi.Services;

public class GameService
{
    private readonly GameDbContext _context;
    private const int Rows = 50;
    private const int Cols = 50;

    public GameService(GameDbContext context)
    {
        _context = context;
    }

    public async Task<List<Cell>> GetIterationAsync(int iteration, string uuid)
    {
        var cells = await _context.Cells
            .Where(c => c.Iteration == iteration && c.Uuid == uuid)
            .ToListAsync();

        if (cells.Count == 0 && iteration > 0)
        {
            var previousCells = await _context.Cells
                .Where(c => c.Iteration == iteration - 1 && c.Uuid == uuid)
                .ToListAsync();

            if (previousCells.Count == 0)
            {
                throw new Exception("Previous iteration not found");
            }

            cells = CalculateNextIteration(previousCells, iteration, uuid);
            await _context.Cells.AddRangeAsync(cells);
            await _context.SaveChangesAsync();
        }

        return cells;
    }

    private List<Cell> CalculateNextIteration(List<Cell> previousCells, int newIteration, string uuid)
    {
        var newCells = new List<Cell>();
        var aliveCells = previousCells.Where(c => c.IsAlive).ToList();

        for (int x = 0; x < Rows; x++)
        {
            for (int y = 0; y < Cols; y++)
            {
                int neighbors = CountNeighbors(aliveCells, x, y);
                bool wasAlive = aliveCells.Any(c => c.X == x && c.Y == y);
                bool isAlive = (wasAlive && (neighbors == 2 || neighbors == 3)) || (!wasAlive && neighbors == 3);

                if (isAlive)
                {
                    newCells.Add(new Cell { Iteration = newIteration, X = x, Y = y, IsAlive = true, Uuid=uuid });
                }
            }
        }

        return newCells;
    }

    private int CountNeighbors(List<Cell> aliveCells, int x, int y)
    {
        int count = 0;
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                int nx = (x + dx + Rows) % Rows;
                int ny = (y + dy + Cols) % Cols;
                if (aliveCells.Any(c => c.X == nx && c.Y == ny))
                {
                    count++;
                }
            }
        }
        return count;
    }

    public async Task InitializeGameAsync(List<Cell> initialCells)
    {
        await _context.Cells.AddRangeAsync(initialCells);
        await _context.SaveChangesAsync();
    }
}