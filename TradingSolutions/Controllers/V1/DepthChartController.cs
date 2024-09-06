using Microsoft.AspNetCore.Mvc;
using TradingSolutions.Application.Models;
using TradingSolutions.Application.Processors;
using TradingSolutions.Application.Requests;

namespace TradingSolutions.Controllers.V1
{
    [ApiController]
    [Route("[controller]")]
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
        //todo from body
        public ActionResult AddPlayerToDepthChart(AddPlayerRequest request)
        {
            _playerProcessor.AddPlayerToDepthChart(request.Position, request.Player, request.PositionDepth);
            return Ok();
        }

        [HttpDelete]
        public ActionResult<IEnumerable<Player>> RemovePlayerToDepthChart(RemovePlayerRequest request)
        {
            var playerList = _playerProcessor.RemovePlayerToDepthChart(request.Position, request.Player);
            return Ok(playerList);
        }

        [HttpGet("backups")]
        public ActionResult<IEnumerable<Player>> GetBackups(GetPlayerBackupsRequest request)
        {
            var backups = _playerProcessor.GetBackups(request.Position, request.Player);
            return Ok(backups);
        }

        [HttpGet]
        public string GetFullDepthChart()
        {
            var depthChart = _playerProcessor.GetFullDepthChart();
            return depthChart;
        }
    }
}
