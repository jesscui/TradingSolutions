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
        void AddPlayersToDepthChart(string position, IEnumerable<AddNflPlayerRequest> players);
        ExecutionResult AddPlayerToDepthChart(string position, Player player, int positionDepth);
        IEnumerable<Player> GetBackups(string position, Player player);
        IDictionary<string, List<Player>> GetFullDepthChart();
        IEnumerable<Player> RemovePlayerFromDepthChart(string position, Player player);
    }

    public class NflChartProcessor : INflChartProcessor
    {
        private readonly INflDepthChartRepository _repository;

        public NflChartProcessor(INflDepthChartRepository repository)
        {
            _repository = repository;

        }

        public void AddPlayersToDepthChart(string position, IEnumerable<AddNflPlayerRequest> request)
        {
            foreach (var player in request)
            {
                _repository.AddPlayer(position, player.Player, player.PositionDepth);
            }
        }

        public ExecutionResult AddPlayerToDepthChart(string position, Player player, int positionDepth)
        {
            var result = new ExecutionResult
            {
                IsSuccess = true,
                IsValid = true
            };
            var positionChart = _repository.GetPositionDepthChart(position);

            var currentPositionDepth = positionChart.FindIndex(x => x.Number == player.Number);

            // doesn't exist - insert normally
            if (currentPositionDepth == -1)
            {
                if (positionDepth > positionChart.Count)
                {
                    result.IsSuccess = false;
                    result.IsValid = false;
                    result.ErrorDetails = [$"New position depth '{positionDepth}' exceeds current position chart depth '{positionChart.Count}'"];
                    return result;
                }
                _repository.AddPlayer(position, player, positionDepth);
                return result;
            }
            else
            {
                // updating to current position - do nothing;
                if (currentPositionDepth == positionDepth)
                {
                    return result;
                }

                if (positionDepth >= positionChart.Count)
                {
                    result.IsSuccess = false;
                    result.IsValid = false;
                    result.ErrorDetails = [$"Player number '{player.Number}' already exists at position '{currentPositionDepth}'. Cannot move to '{positionDepth}' as it would exceed current position chart depth '{positionChart.Count - 1}'"];
                    return result;
                }

                //move existing player to new position
                _repository.MovePlayerPosition(position, positionDepth, player);
                return result;
            }
        }

        public IEnumerable<Player> GetBackups(string position, Player player)
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

        public IDictionary<string, List<Player>> GetFullDepthChart()
            => _repository.GetFullDepthChart();

        public IEnumerable<Player> RemovePlayerFromDepthChart(string position, Player player)
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
