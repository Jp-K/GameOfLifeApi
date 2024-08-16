using Microsoft.AspNetCore.Mvc;
using GameOfLifeApi.Models;
using GameOfLifeApi.Services;

namespace GameOfLifeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly GameService _gameService;

    public GameController(GameService gameService)
    {
        _gameService = gameService;
    }

    [HttpGet("{iteration}/{uuid}/")]
    public async Task<ActionResult<List<Cell>>> GetIteration(int iteration, string uuid)
    {
        try
        {
            var cells = await _gameService.GetIterationAsync(iteration, uuid);
            return Ok(cells);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("initialize")]
    public async Task<IActionResult> InitializeGame([FromBody] List<Cell> initialCells)
    {
        await _gameService.InitializeGameAsync(initialCells);
        return Ok();
    }
}