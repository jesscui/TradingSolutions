using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingSolutions.Application.Enums;
using TradingSolutions.Application.Models;
using TradingSolutions.Application.Requests;

namespace TradingSolutions.Application.Processors
{
    public interface IPlayerProcessor
    {
        //todo concurrent dictionary
        void AddPlayersToDepthChart(NhlPositions position, IEnumerable<AddPlayerRequest> players);
        ExecutionResult AddPlayerToDepthChart(NhlPositions position, Player player, int positionDepth = -1);
        IEnumerable<Player> GetBackups(NhlPositions position, Player player);
        IDictionary<NhlPositions, List<Player>> GetFullDepthChart();
        IEnumerable<Player> RemovePlayerToDepthChart(NhlPositions position, Player player);
    }

    public class PlayerProcessor : IPlayerProcessor
    {
        private readonly Dictionary<NhlPositions, List<Player>> _depthChart = [];

        public void AddPlayersToDepthChart(NhlPositions position, IEnumerable<AddPlayerRequest> request)
        {
            var result = new ExecutionResult();
            if (!_depthChart.TryGetValue(position, out var positionChart))
            {
                positionChart = [];
            }
            foreach (var player in request)
            {
                positionChart.Insert(player.PositionDepth, player.Player);

            }
            _depthChart[position] = positionChart;
        }

        public ExecutionResult AddPlayerToDepthChart(NhlPositions position, Player player, int newPositionDepth = -1)
        {
            var result = new ExecutionResult();
            if (!_depthChart.TryGetValue(position, out var positionChart))
            {
                positionChart = [];
            }

            var currentPositionDepth = positionChart.FindIndex(x => x.Number == player.Number);

            // doesn't exist - insert normally
            if (currentPositionDepth == -1)
            {
                if (newPositionDepth > positionChart.Count)
                {
                    result.IsValid = false;
                    result.ErrorDetails = [$"New position depth '{newPositionDepth}' exceeds current position chart depth '{positionChart.Count}'"];
                    return result;
                }
                if (newPositionDepth == -1)
                {
                    positionChart.Add(player);
                }
                else
                {
                    positionChart.Insert(newPositionDepth, player);
                }
                _depthChart[position] = positionChart;
                result.IsSuccess = true;
                result.IsValid = true;
                return result;
            }
            else
            {
                // updating to current position - do nothing;
                if (currentPositionDepth == newPositionDepth)
                {
                    result.IsSuccess = true;
                    result.IsValid = true;
                    return result;
                }

                //move existing player to new position
                if (newPositionDepth >= positionChart.Count)
                {
                    result.IsValid = false;
                    result.ErrorDetails = [$"Player number '{player.Number}' already exists at position '{currentPositionDepth}'. Cannot move to '{newPositionDepth}' as it would exceed current position chart depth '{positionChart.Count - 1}'"];
                    return result;
                }
                positionChart.RemoveAt(currentPositionDepth);
                positionChart.Insert(newPositionDepth, player);
                result.IsSuccess = true;
                result.IsValid = true;
                return result;
            }
        }

        public IEnumerable<Player> GetBackups(NhlPositions position, Player player)
        {
            if (!_depthChart.TryGetValue(position, out var positionChart))
            {
                return Enumerable.Empty<Player>();
            }
            var playerAtPosition = positionChart.FirstOrDefault(x => x.Number == player.Number);
            if (playerAtPosition == null) return Enumerable.Empty<Player>();
            if (positionChart.Last().Number == playerAtPosition.Number) return Enumerable.Empty<Player>();

            var backups = new List<Player>();

            var currentPlayerPositionDepth = positionChart.FindIndex(x => x.Number == player.Number);

            for (int i = currentPlayerPositionDepth + 1; i < positionChart.Count; i++)
            {
                backups.Add(positionChart[i]);
            }
            return backups;
        }

        public IDictionary<NhlPositions, List<Player>> GetFullDepthChart()
            => _depthChart;

        public IEnumerable<Player> RemovePlayerToDepthChart(NhlPositions position, Player player)
        {
            if (!_depthChart.TryGetValue(position, out var positionChart))
            {
                return Enumerable.Empty<Player>();
            }

            var playerAtPosition = positionChart.FirstOrDefault(x => x.Number == player.Number);
            if (playerAtPosition == null) return Enumerable.Empty<Player>();
            positionChart.Remove(playerAtPosition);
            return new List<Player> { playerAtPosition };
        }
    }
}
