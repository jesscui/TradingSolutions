using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using TradingSolutions.Application.Enums;
using TradingSolutions.Application.Models;
using TradingSolutions.Application.Processors;
using TradingSolutions.Application.Requests;

namespace TradingSolutions.Controllers.V1
{
    [ApiVersion("1")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class DepthChartController : ControllerBase
    {

        private readonly ILogger<DepthChartController> _logger;
        private readonly IPlayerProcessor _playerProcessor;

        public DepthChartController(IPlayerProcessor processor, ILogger<DepthChartController> logger)
        {
            _logger = logger;
            _playerProcessor = processor;
        }

        [HttpPut]
        public ActionResult AddPlayerToDepthChart([FromBody] AddPlayerRequest request)
        {
            var result = _playerProcessor.AddPlayerToDepthChart(request.Position, request.Player, request.PositionDepth);
            if (!result.IsValid)
            {
                return BadRequest(result.ErrorDetails);
            }
            return Ok();
        }

        [HttpPut("Position/{position}")]
        public ActionResult AddPlayersToDepthChart(NhlPositions position, [FromBody] IEnumerable<AddPlayerRequest> players)
        {
            _playerProcessor.AddPlayersToDepthChart(position, players);
            return Ok();
        }

        [HttpDelete]
        public ActionResult<IEnumerable<Player>> RemovePlayerFromDepthChart([FromBody] RemovePlayerRequest request)
        {
            var playerList = _playerProcessor.RemovePlayerToDepthChart(request.Position, request.Player);
            return Ok(playerList);
        }

        [HttpGet("backups/position/{position}")]
        public ActionResult<IEnumerable<Player>> GetBackups(int position, [FromQuery] int playerNumber, [FromQuery] string playerName)
        {
            if (string.IsNullOrEmpty(playerName) || playerNumber < 0) return BadRequest();
            var player = new Player
            {
                Name = playerName,
                Number = playerNumber
            };
            var backups = _playerProcessor.GetBackups((NhlPositions)position, player);
            return Ok(backups);
        }

        [HttpGet]
        public IDictionary<NhlPositions, List<Player>> GetFullDepthChart()
        {
            var depthChart = _playerProcessor.GetFullDepthChart();
            return depthChart;
        }
    }
}
