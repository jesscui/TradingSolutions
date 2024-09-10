using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingSolutions.Application.Enums;
using TradingSolutions.Application.Models;

namespace TradingSolutions.Application.Repositories
{
    public interface IDepthChartRepository
    {
        List<Player> GetPositionDepthChart(NhlPositions position);
        void AddPlayer(NhlPositions position, Player player, int positionDepth);
        void MovePlayerPosition(NhlPositions position, int currentPositionDepth, int newPositionDepth, Player player);
        Dictionary<NhlPositions, List<Player>> GetFullDepthChart();
    }

    public class DepthChartRepository : IDepthChartRepository
    {
        private readonly Dictionary<NhlPositions, List<Player>> _depthCharts = [];

        public List<Player> GetPositionDepthChart(NhlPositions position)
            => GetPositionDepthChartInternal(position);

        public void AddPlayer(NhlPositions position, Player player, int newPositionDepth)
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

        public void MovePlayerPosition(NhlPositions position, int currentPositionDepth, int newPositionDepth, Player player)
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
        public Dictionary<NhlPositions, List<Player>> GetFullDepthChart()
            => _depthCharts;

        private List<Player> GetPositionDepthChartInternal(NhlPositions position)
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
