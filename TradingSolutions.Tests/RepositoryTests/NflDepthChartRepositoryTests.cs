using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingSolutions.Application.Enums;
using TradingSolutions.Application.Models;
using TradingSolutions.Application.Processors;
using TradingSolutions.Application.Repositories;

namespace TradingSolutions.Tests.RepositoryTests
{
    public class NflDepthChartRepositoryTests
    {
        [Fact]
        public void AddPlayer_AddToEnd()
        {
            var position = NflPosition.QB.ToString();
            var repository = new NflDepthChartRepository();
            var player0 = new Player()
            {
                Name = "Tom",
                Number = 13
            };
            var player1 = new Player()
            {
                Name = "Blaine",
                Number = 2
            };

            var chart = repository.GetPositionDepthChart(position);
            chart.Add(player0);
            chart.Add(player1);
            Assert.Equal(2, chart.Count);

            var player2 = new Player()
            {
                Name = "Kyle",
                Number = 20
            };
            repository.AddPlayer(position, player2, -1);
            Assert.Equal(3, chart.Count);

            var player = chart.LastOrDefault();
            Assert.NotNull(player);
            Assert.Equal(player2.Name, player.Name);
            Assert.Equal(player2.Number, player.Number);
        }

        [Fact]
        public void AddPlayer_AddAtIndex()
        {
            var position = NflPosition.QB.ToString();
            var repository = new NflDepthChartRepository();
            var player0 = new Player()
            {
                Name = "Tom",
                Number = 13
            };
            var player1 = new Player()
            {
                Name = "Blaine",
                Number = 2
            };

            var chart = repository.GetPositionDepthChart(position);
            chart.Add(player0);
            chart.Add(player1);
            Assert.Equal(2, chart.Count);
            var player = chart.LastOrDefault();
            Assert.NotNull(player);
            Assert.Equal(player1.Name, player.Name);
            Assert.Equal(player1.Number, player.Number);

            var player2 = new Player()
            {
                Name = "Kyle",
                Number = 20
            };
            var index = 1;
            repository.AddPlayer(position, player2, index);
            chart = repository.GetPositionDepthChart(position);
            Assert.Equal(3, chart.Count);

            var insertedPlayer = chart[index];
            Assert.NotNull(insertedPlayer);
            Assert.Equal(player2.Name, insertedPlayer.Name);
            Assert.Equal(player2.Number, insertedPlayer.Number);

            var lastPlayer = chart.LastOrDefault();
            Assert.NotNull(lastPlayer);
            Assert.Equal(player1.Name, lastPlayer.Name);
            Assert.Equal(player1.Number, lastPlayer.Number);
        }

        [Fact]
        public void MovePlayerPosition_MoveToEnd()
        {
            var position = NflPosition.QB.ToString();
            var repository = new NflDepthChartRepository();
            var player0 = new Player() { Name = "Tom", Number = 13 };
            var player1 = new Player() { Name = "Blaine", Number = 2 };
            var player2 = new Player() { Name = "Kyle", Number = 20 };

            var chart = repository.GetPositionDepthChart(position);
            chart.Add(player0);
            chart.Add(player1);
            chart.Add(player2);
            Assert.Equal(player0, chart[0]);
            Assert.Equal(player1, chart[1]);
            Assert.Equal(player2, chart[2]);

            repository.MovePlayerPosition(position, -1, player0);
            chart = repository.GetPositionDepthChart(position);
            Assert.Equal(player1, chart[0]);
            Assert.Equal(player2, chart[1]);
            Assert.Equal(player0, chart[2]);

            repository.MovePlayerPosition(position, -1, player2);
            chart = repository.GetPositionDepthChart(position);
            Assert.Equal(player1, chart[0]);
            Assert.Equal(player0, chart[1]);
            Assert.Equal(player2, chart[2]);
        }

        [Fact]
        public void MovePlayerPosition_MoveToIndex()
        {
            var position = NflPosition.QB.ToString();
            var repository = new NflDepthChartRepository();
            var player0 = new Player() { Name = "Tom", Number = 13 };
            var player1 = new Player() { Name = "Blaine", Number = 2 };
            var player2 = new Player() { Name = "Kyle", Number = 20 };
            var player3 = new Player() { Name = "Test", Number = 8 };

            var chart = repository.GetPositionDepthChart(position);
            chart.Add(player0);
            chart.Add(player1);
            chart.Add(player2);
            chart.Add(player3);
            Assert.Equal(player0, chart[0]);
            Assert.Equal(player1, chart[1]);
            Assert.Equal(player2, chart[2]);
            Assert.Equal(player3, chart[3]);

            repository.MovePlayerPosition(position, 2, player0);
            chart = repository.GetPositionDepthChart(position);
            Assert.Equal(player1, chart[0]);
            Assert.Equal(player2, chart[1]);
            Assert.Equal(player0, chart[2]);
            Assert.Equal(player3, chart[3]);

            repository.MovePlayerPosition(position, 0, player3);
            chart = repository.GetPositionDepthChart(position);
            Assert.Equal(player3, chart[0]);
            Assert.Equal(player1, chart[1]);
            Assert.Equal(player2, chart[2]);
            Assert.Equal(player0, chart[3]);

            repository.MovePlayerPosition(position, 3, player1);
            chart = repository.GetPositionDepthChart(position);
            Assert.Equal(player3, chart[0]);
            Assert.Equal(player2, chart[1]);
            Assert.Equal(player0, chart[2]);
            Assert.Equal(player1, chart[3]);
        }

        [Fact]
        public void GetFullDepthChart_EmptyDepthChart_ReturnEmptyList()
        {
            var repository = new NflDepthChartRepository();

            var depthChart = repository.GetFullDepthChart();
            Assert.Empty(depthChart);
        }

        [Fact]
        public void GetFullDepthChart_MultipleDepthCharts_ReturnList()
        {
            var qbPosition = NflPosition.QB.ToString();
            var lwrPosition = NflPosition.LWR.ToString();
            var repository = new NflDepthChartRepository();

            var qbPlayer1 = new Player { Name = "Tom Brady", Number = 12 };
            var qbPlayer2 = new Player { Name = "Blaine", Number = 11 };
            var lwrPlayer1 = new Player { Name = "Jon", Number = 20 };
            var lwrPlayer2 = new Player { Name = "Jim", Number = 5 };

            var qbChart = repository.GetPositionDepthChart(qbPosition);
            qbChart.Add(qbPlayer1);
            qbChart.Add(qbPlayer2);
            var lwrChart = repository.GetPositionDepthChart(lwrPosition);
            lwrChart.Add(lwrPlayer1);
            lwrChart.Add(lwrPlayer2);

            var depthChart = repository.GetFullDepthChart();
            Assert.NotEmpty(depthChart);

            var chart = depthChart[NflPosition.QB.ToString()];
            Assert.NotEmpty(chart);
            Assert.Equal(2, chart.Count);
            Assert.Equal(qbPlayer1, chart[0]);
            Assert.Equal(qbPlayer2, chart[1]);

            chart = depthChart[NflPosition.LWR.ToString()];
            Assert.NotEmpty(chart);
            Assert.Equal(2, chart.Count);
            Assert.Equal(lwrPlayer1, chart[0]);
            Assert.Equal(lwrPlayer2, chart[1]);
        }

        [Fact]
        public void RemovePlayer_PlayerRemoved()
        {
            var position = NflPosition.QB.ToString();
            var repository = new NflDepthChartRepository();
            var player0 = new Player() { Name = "Tom", Number = 13 };
            var player1 = new Player() { Name = "Blaine", Number = 2 };
            var player2 = new Player() { Name = "Kyle", Number = 20 };
            var player3 = new Player() { Name = "Test", Number = 8 };

            var chart = repository.GetPositionDepthChart(position);
            chart.Add(player0);
            chart.Add(player1);
            chart.Add(player2);
            chart.Add(player3);
            Assert.Equal(4, chart.Count);

            repository.RemovePlayer(position, player0);
            Assert.Equal(3, chart.Count);
            Assert.Equal(-1, chart.FindIndex(x => x.Number == player0.Number));
        }
    }
}
