using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingSolutions.Application.Enums;
using TradingSolutions.Application.Models;

namespace TradingSolutions.Application.Repositories
{
    public interface INflDepthChartRepository
    {
        List<Player> GetPositionDepthChart(string position);
        void AddPlayer(string position, Player player, int positionDepth);
        void MovePlayerPosition(string position, int newPositionDepth, Player player);
        ConcurrentDictionary<string, List<Player>> GetFullDepthChart();
        void RemovePlayer(string position, Player player);
    }

    public class NflDepthChartRepository : INflDepthChartRepository
    {
        // To add all other NFL teams - add a tuple key of (string Team, string Position)
        // eg. private readonly Dictionary<(string, string), List<Player>> _depthCharts = [];
        // would've implemented this however that requires all the methods to accept an extra argument of team,
        // eg. getBackups("Buccaneers", "QB", KyleTrask) instead of getBackups("QB", KyleTrask) 
        private readonly ConcurrentDictionary<string, List<Player>> _depthCharts = [];

        public List<Player> GetPositionDepthChart(string position)
            => GetPositionDepthChartInternal(position);

        public void AddPlayer(string position, Player player, int newPositionDepth)
        {
            var positionChart = GetPositionDepthChartInternal(position);

            if (newPositionDepth < 0)
            {
                positionChart.Add(player);
            }
            else
            {
                positionChart.Insert(newPositionDepth, player);
            }
        }

        public void MovePlayerPosition(string position, int newPositionDepth, Player player)
        {
            var positionChart = GetPositionDepthChartInternal(position);

            var currentPositionDepth = positionChart.FindIndex(x => x.Number == player.Number);

            if (currentPositionDepth != -1)
            {
                positionChart.RemoveAt(currentPositionDepth);
            }
            if (newPositionDepth < 0)
            {
                positionChart.Add(player);
            }
            else
            {
                positionChart.Insert(newPositionDepth, player);
            }

        }

        public ConcurrentDictionary<string, List<Player>> GetFullDepthChart()
            => _depthCharts;

        public void RemovePlayer(string position, Player player)
        {
            var chart = GetPositionDepthChartInternal(position);
            chart.Remove(player);
        }

        private List<Player> GetPositionDepthChartInternal(string position)
        {
            if (!_depthCharts.TryGetValue(position, out var positionChart))
            {
                positionChart = [];
                _depthCharts[position] = positionChart;
            }
            return positionChart;
        }
    }
}
