using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using TradingSolutions.Application.Enums;
using TradingSolutions.Application.Models;
using TradingSolutions.Application.Processors;
using TradingSolutions.Application.Requests.Nfl;

namespace TradingSolutions.Controllers.V1
{
    [ApiVersion("1")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class NflDepthChartController : ControllerBase
    {

        private readonly ILogger<NflDepthChartController> _logger;
        private readonly INflChartProcessor _playerProcessor;

        public NflDepthChartController(INflChartProcessor processor, ILogger<NflDepthChartController> logger)
        {
            _logger = logger;
            _playerProcessor = processor;
        }

        [HttpPut]
        public ActionResult AddPlayerToDepthChart([FromBody] AddNflPlayerRequest request)
        {
            var result = _playerProcessor.AddPlayerToDepthChart(request.Position, request.Player, request.PositionDepth);
            if (!result.IsValid)
            {
                return BadRequest(result.ErrorDetails);
            }
            return Ok();
        }

        [HttpGet]
        public IDictionary<string, List<Player>> GetFullDepthChart()
        {
            var depthChart = _playerProcessor.GetFullDepthChart();
            return depthChart;
        }

        [HttpGet("backups/position/{position}")]
        public ActionResult<IEnumerable<Player>> GetBackups(string position, [FromQuery] int playerNumber, [FromQuery] string playerName)
        {
            if (!Enum.IsDefined(typeof(NflPosition), position)) return BadRequest("Invalid position provided");
            if (string.IsNullOrEmpty(playerName)) return BadRequest("Please provide a valid player name");
            if (playerNumber < 0) return BadRequest("Please provide a valid player number");
            var player = new Player
            {
                Name = playerName,
                Number = playerNumber
            };
            var backups = _playerProcessor.GetBackups(position, player);
            return Ok(backups);
        }

        [HttpDelete]
        public ActionResult<IEnumerable<Player>> RemovePlayerFromDepthChart([FromBody] RemoveNflPlayerRequest request)
        {
            var playerList = _playerProcessor.RemovePlayerFromDepthChart(request.Position, request.Player);
            return Ok(playerList);
        }

    }
}
