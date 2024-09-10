using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingSolutions.Application.Enums;
using TradingSolutions.Application.Models;
using TradingSolutions.Application.Repositories;
using TradingSolutions.Application.Requests.Nfl;

namespace TradingSolutions.Application.Processors
{
    public interface INflChartProcessor
    {
        //todo concurrent dictionary
        void AddPlayersToDepthChart(NflPosition position, IEnumerable<AddNflPlayerRequest> players);
        ExecutionResult AddPlayerToDepthChart(AddNflPlayerRequest request);
        IEnumerable<Player> GetBackups(NflPosition position, Player player);
        IDictionary<NflPosition, List<Player>> GetFullDepthChart();
        IEnumerable<Player> RemovePlayerFromDepthChart(NflPosition position, Player player);
    }

    public class NflChartProcessor : INflChartProcessor
    {
        private readonly INflDepthChartRepository _repository;

        public NflChartProcessor(INflDepthChartRepository repository)
        {
            _repository = repository;

        }

        public void AddPlayersToDepthChart(NflPosition position, IEnumerable<AddNflPlayerRequest> request)
        {
            foreach (var player in request)
            {
                _repository.AddPlayer(position, player.Player, player.PositionDepth);
            }
        }

        public ExecutionResult AddPlayerToDepthChart(AddNflPlayerRequest request)
        {
            var result = new ExecutionResult
            {
                IsSuccess = true,
                IsValid = true
            };
            var positionChart = _repository.GetPositionDepthChart(request.Position);

            var currentPositionDepth = positionChart.FindIndex(x => x.Number == request.Player.Number);

            // doesn't exist - insert normally
            if (currentPositionDepth == -1)
            {
                if (request.PositionDepth > positionChart.Count)
                {
                    result.IsSuccess = false;
                    result.IsValid = false;
                    result.ErrorDetails = [$"New position depth '{request.PositionDepth}' exceeds current position chart depth '{positionChart.Count}'"];
                    return result;
                }
                _repository.AddPlayer(request.Position, request.Player, request.PositionDepth);
                return result;
            }
            else
            {
                // updating to current position - do nothing;
                if (currentPositionDepth == request.PositionDepth)
                {
                    return result;
                }

                if (request.PositionDepth >= positionChart.Count)
                {
                    result.IsSuccess = false;
                    result.IsValid = false;
                    result.ErrorDetails = [$"Player number '{request.Player.Number}' already exists at position '{currentPositionDepth}'. Cannot move to '{request.PositionDepth}' as it would exceed current position chart depth '{positionChart.Count - 1}'"];
                    return result;
                }

                //move existing player to new position
                _repository.MovePlayerPosition(request.Position, request.PositionDepth, request.Player);
                return result;
            }
        }

        public IEnumerable<Player> GetBackups(NflPosition position, Player player)
        {
            var positionChart = _repository.GetPositionDepthChart(position);

            if (positionChart.Count == 0)
            {
                return Enumerable.Empty<Player>();
            }
            var currentPlayerPositionDepth = positionChart.FindIndex(x => x.Number == player.Number);
            if (currentPlayerPositionDepth == -1) return Enumerable.Empty<Player>();

            var backups = new List<Player>();
            for (int i = currentPlayerPositionDepth + 1; i < positionChart.Count; i++)
            {
                backups.Add(positionChart[i]);
            }
            return backups;
        }

        public IDictionary<NflPosition, List<Player>> GetFullDepthChart()
            => _repository.GetFullDepthChart();

        public IEnumerable<Player> RemovePlayerFromDepthChart(NflPosition position, Player player)
        {
            var positionChart = _repository.GetPositionDepthChart(position);
            if (positionChart.Count == 0)
            {
                return Enumerable.Empty<Player>();
            }

            var playerAtPosition = positionChart.FirstOrDefault(x => x.Number == player.Number);
            if (playerAtPosition == null) return Enumerable.Empty<Player>();
            _repository.RemovePlayer(position, playerAtPosition);
            return new List<Player> { playerAtPosition };
        }
    }
}
