using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingSolutions.Application.Enums;
using TradingSolutions.Application.Models;

namespace TradingSolutions.Application.Processors
{
    public interface IPlayerProcessor
    {

        //todo - async?
        void AddPlayerToDepthChart(NhlPositions position, Player player, int positionDepth = -1);
        IEnumerable<Player> GetBackups(NhlPositions position, Player player);
        string GetFullDepthChart();
        IEnumerable<Player> RemovePlayerToDepthChart(NhlPositions position, Player player);
    }

    //todo - add git
    public class PlayerProcessor : IPlayerProcessor
    {
        //list, linkedlist, sorted list
        SortedList<int, Player> _players;
        private IDictionary<NhlPositions, List<Player>> _depthChart = new Dictionary<NhlPositions, List<Player>>();
        //private IList<Player> _depthChart = new List<Player>();

        public void AddPlayerToDepthChart(NhlPositions position, Player player, int positionDepth = -1)
        {
            var positionChart = _depthChart[position];
            if (positionDepth == -1)
            {
                //check if posiition chart can be null
                positionChart.Add(player);
                return;
            }

            positionChart.Insert(positionDepth, player);
        }

        public IEnumerable<Player> GetBackups(NhlPositions position, Player player)
        {
            var positionChart = _depthChart[position];
            if (!positionChart.Contains(player)) return Enumerable.Empty<Player>();
            if (positionChart.Last() == player) return Enumerable.Empty<Player>();
            var backups = new List<Player>();

            var currentPlayerPositionDepth = positionChart.IndexOf(player);

            for (int i = currentPlayerPositionDepth + 1; i < positionChart.Count; i++)
            {
                backups.Add(positionChart[i]);
            }
            return backups;
        }

        public string GetFullDepthChart()
        {
            string x = "";
            foreach (var key in _depthChart.Keys)
            {
                x.Concat(key.ToString() + " - ");
                foreach (Player player in _depthChart[key])
                {
                    var formattedPlayerDetails = $"(#{player.Number}, {player.Name}),";
                    x.Concat(formattedPlayerDetails);
                }
                x.Concat("\n");
            }
            return x;
        }

        public IEnumerable<Player> RemovePlayerToDepthChart(NhlPositions position, Player player)
        {
            var positionChart = _depthChart[position];
            if (!positionChart.Contains(player)) return Enumerable.Empty<Player>();
            positionChart.Remove(player);
            return new List<Player> { player };
        }
    }
}
