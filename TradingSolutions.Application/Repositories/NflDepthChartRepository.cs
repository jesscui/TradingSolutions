using System;
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
        List<Player> GetPositionDepthChart(NflPosition position);
        void AddPlayer(NflPosition position, Player player, int positionDepth);
        void MovePlayerPosition(NflPosition position, int currentPositionDepth, int newPositionDepth, Player player);
        Dictionary<NflPosition, List<Player>> GetFullDepthChart();
    }

    public class NflDepthChartRepository : INflDepthChartRepository
    {
        // To add all other NFL teams - add a tuple key of (enum Team, enum Position)
        // eg. private readonly Dictionary<(enum, enum), List<Player>> _depthCharts = [];
        // would've implemented this however that requires all the methods to accept an extra argument of team,
        // eg. getBackups("Buccaneers", "QB", KyleTrask) instead of getBackups("QB", KyleTrask) 

        private readonly Dictionary<NflPosition, List<Player>> _depthCharts = [];

        public List<Player> GetPositionDepthChart(NflPosition position)
            => GetPositionDepthChartInternal(position);

        public void AddPlayer(NflPosition position, Player player, int newPositionDepth)
        {
            var positionChart = GetPositionDepthChartInternal(position);

            if (newPositionDepth == -1)
            {
                positionChart.Add(player);
            }
            else
            {
                positionChart.Insert(newPositionDepth, player);
            }
        }

        public void MovePlayerPosition(NflPosition position, int currentPositionDepth, int newPositionDepth, Player player)
        {
            var positionChart = GetPositionDepthChartInternal(position);
            positionChart.RemoveAt(currentPositionDepth);
            if (newPositionDepth == -1)
            {
                positionChart.Add(player);
            }
            else
            {
                positionChart.Insert(newPositionDepth, player);
            }

        }
        public Dictionary<NflPosition, List<Player>> GetFullDepthChart()
            => _depthCharts;

        private List<Player> GetPositionDepthChartInternal(NflPosition position)
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
