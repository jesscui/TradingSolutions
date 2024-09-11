using Moq;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingSolutions.Application.Enums;
using TradingSolutions.Application.Models;
using TradingSolutions.Application.Processors;
using TradingSolutions.Application.Repositories;
using TradingSolutions.Application.Requests.Nfl;

namespace TradingSolutions.Tests.ProcessorTests
{
    public class NflChartProcessorTests
    {
        private readonly string _position = NflPosition.QB.ToString();

        [Fact]
        public void AddPlayerToDepthChart_DoesNotExist_InvalidPositionDepth_ReturnError()
        {
            var mockRepository = new Mock<INflDepthChartRepository>();
            mockRepository.Setup(x => x.GetPositionDepthChart(It.IsAny<string>())).Returns(new List<Player>());
            var processor = new NflChartProcessor(mockRepository.Object);

            var request = new AddNflPlayerRequest
            {
                Position = _position,
                Player = new Player
                {
                    Name = "Tom Brady",
                    Number = 12
                },
                PositionDepth = 1
            };
            var result = processor.AddPlayerToDepthChart(request.Position, request.Player, request.PositionDepth);
            Assert.False(result.IsSuccess);
            Assert.False(result.IsValid);
            mockRepository.Verify(x => x.AddPlayer(It.IsAny<string>(), It.IsAny<Player>(), It.IsAny<int>()), Times.Never);
            mockRepository.Verify(x => x.MovePlayerPosition(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Player>()), Times.Never);
        }

        [Fact]
        public void AddPlayerToDepthChart_DoesNotExist_ValidPositionDepth_SuccessfullyAdded()
        {
            var mockRepository = new Mock<INflDepthChartRepository>();
            mockRepository.Setup(x => x.GetPositionDepthChart(It.IsAny<string>())).Returns(new List<Player>());
            var processor = new NflChartProcessor(mockRepository.Object);

            var request = new AddNflPlayerRequest
            {
                Position = _position,
                Player = new Player
                {
                    Name = "Tom Brady",
                    Number = 12
                },
                PositionDepth = 0
            };
            var result = processor.AddPlayerToDepthChart(request.Position, request.Player, request.PositionDepth);
            mockRepository.Verify(x => x.AddPlayer(It.IsAny<string>(), It.IsAny<Player>(), It.IsAny<int>()), Times.Once);
            mockRepository.Verify(x => x.MovePlayerPosition(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Player>()), Times.Never);
            Assert.True(result.IsSuccess);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void AddPlayerToDepthChart_AlreadyExists_SamePositionDepth_ReturnSuccess()
        {
            var player = new Player
            {
                Name = "Tom Brady",
                Number = 12
            };
            var mockRepository = new Mock<INflDepthChartRepository>();
            mockRepository.Setup(x => x.GetPositionDepthChart(It.IsAny<string>())).Returns(new List<Player> { player });
            var processor = new NflChartProcessor(mockRepository.Object);

            var request = new AddNflPlayerRequest
            {
                Position = _position,
                Player = player,
                PositionDepth = 0
            };
            var result = processor.AddPlayerToDepthChart(request.Position, request.Player, request.PositionDepth);
            mockRepository.Verify(x => x.AddPlayer(It.IsAny<string>(), It.IsAny<Player>(), It.IsAny<int>()), Times.Never);
            mockRepository.Verify(x => x.MovePlayerPosition(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Player>()), Times.Never);
            Assert.True(result.IsSuccess);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void AddPlayerToDepthChart_AlreadyExists_InvalidPositionDepth_ReturnSuccess()
        {
            var player = new Player
            {
                Name = "Tom Brady",
                Number = 12
            };
            var mockRepository = new Mock<INflDepthChartRepository>();
            mockRepository.Setup(x => x.GetPositionDepthChart(It.IsAny<string>())).Returns(new List<Player> { player });
            var processor = new NflChartProcessor(mockRepository.Object);

            var request = new AddNflPlayerRequest
            {
                Position = _position,
                Player = player,
                PositionDepth = 1
            };
            var result = processor.AddPlayerToDepthChart(request.Position, request.Player, request.PositionDepth);
            mockRepository.Verify(x => x.AddPlayer(It.IsAny<string>(), It.IsAny<Player>(), It.IsAny<int>()), Times.Never);
            mockRepository.Verify(x => x.MovePlayerPosition(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Player>()), Times.Never);
            Assert.False(result.IsSuccess);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void AddPlayerToDepthChart_AlreadyExists_ValidPositionDepth_SuccessfullyMovedPlayerPosition()
        {
            var player1 = new Player
            {
                Name = "Tom Brady",
                Number = 12
            };

            var player2 = new Player
            {
                Name = "Blaine",
                Number = 11
            };

            var mockRepository = new Mock<INflDepthChartRepository>();
            mockRepository.Setup(x => x.GetPositionDepthChart(It.IsAny<string>())).Returns(new List<Player> { player1, player2 });
            var processor = new NflChartProcessor(mockRepository.Object);

            var request = new AddNflPlayerRequest
            {
                Position = _position,
                Player = player1,
                PositionDepth = 1
            };
            var result = processor.AddPlayerToDepthChart(request.Position, request.Player, request.PositionDepth);
            mockRepository.Verify(x => x.AddPlayer(It.IsAny<string>(), It.IsAny<Player>(), It.IsAny<int>()), Times.Never);
            mockRepository.Verify(x => x.MovePlayerPosition(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Player>()), Times.Once);
            Assert.True(result.IsSuccess);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void GetBackups_EmptyPositionChart_ReturnEmptyList()
        {
            var mockRepository = new Mock<INflDepthChartRepository>();
            mockRepository.Setup(x => x.GetPositionDepthChart(It.IsAny<string>())).Returns(new List<Player>());
            var processor = new NflChartProcessor(mockRepository.Object);

            var backups = processor.GetBackups(_position, new Player());
            Assert.Empty(backups);
        }

        [Fact]
        public void GetBackups_PlayerDoesNotExistAtPosition_ReturnEmptyList()
        {
            var player1 = new Player
            {
                Name = "Tom Brady",
                Number = 12
            };
            var player2 = new Player
            {
                Name = "Blaine",
                Number = 11
            };

            var mockRepository = new Mock<INflDepthChartRepository>();
            mockRepository.Setup(x => x.GetPositionDepthChart(It.IsAny<string>())).Returns(new List<Player>() { player1 });
            var processor = new NflChartProcessor(mockRepository.Object);

            var backups = processor.GetBackups(_position, player2);
            Assert.Empty(backups);
        }

        [Fact]
        public void GetBackups_ReturnMultipleBackups()
        {
            var player1 = new Player
            {
                Name = "Tom Brady",
                Number = 12
            };
            var player2 = new Player
            {
                Name = "Blaine",
                Number = 11
            };

            var player3 = new Player()
            {
                Name = "Kyle",
                Number = 20
            };

            var mockRepository = new Mock<INflDepthChartRepository>();
            mockRepository.Setup(x => x.GetPositionDepthChart(It.IsAny<string>())).Returns(new List<Player>() { player1, player2, player3 });
            var processor = new NflChartProcessor(mockRepository.Object);

            var backups = processor.GetBackups(_position, player1);
            Assert.Equal(2, backups.Count());
            var list = backups.ToList();
            Assert.Equal(player2, list[0]);
            Assert.Equal(player3, list[1]);

            backups = processor.GetBackups(_position, player2);
            Assert.Single(backups);
            list = backups.ToList();
            Assert.Equal(player3, list[0]);

            backups = processor.GetBackups(_position, player3);
            Assert.Empty(backups);
        }

        [Fact]
        public void GetFullDepthChart_EmptyDepthChart_ReturnEmptyList()
        {
            var mockRepository = new Mock<INflDepthChartRepository>();
            mockRepository.Setup(x => x.GetFullDepthChart()).Returns(new ConcurrentDictionary<string, List<Player>>());
            var processor = new NflChartProcessor(mockRepository.Object);

            var depthChart = processor.GetFullDepthChart();
            Assert.Empty(depthChart);
        }

        [Fact]
        public void GetFullDepthChart_MultiplePositionsInDepthChart_ReturnList()
        {
            var qbPlayer1 = new Player { Name = "Tom Brady", Number = 12 };
            var qbPlayer2 = new Player { Name = "Blaine", Number = 11 };
            var lwrPlayer1 = new Player { Name = "Jon", Number = 20 };
            var lwrPlayer2 = new Player { Name = "Jim", Number = 5 };

            var qbList = new List<Player> { qbPlayer1, qbPlayer2 };
            var lwrList = new List<Player> { lwrPlayer1, lwrPlayer2 };

            var dict = new ConcurrentDictionary<string, List<Player>>();
            dict.TryAdd(NflPosition.QB.ToString(), qbList);
            dict.TryAdd(NflPosition.LWR.ToString(), lwrList);

            var mockRepository = new Mock<INflDepthChartRepository>();
            mockRepository.Setup(x => x.GetFullDepthChart()).Returns(dict);
            var processor = new NflChartProcessor(mockRepository.Object);

            var depthChart = processor.GetFullDepthChart();
            Assert.NotEmpty(depthChart);

            var qbChart = depthChart[NflPosition.QB.ToString()];
            Assert.NotEmpty(qbChart);
            Assert.Equal(2, qbChart.Count);
            Assert.Equal(qbPlayer1.Name, qbChart[0].Name);
            Assert.Equal(qbPlayer1.Number, qbChart[0].Number);
            Assert.Equal(qbPlayer2.Name, qbChart[1].Name);
            Assert.Equal(qbPlayer2.Number, qbChart[1].Number);

            var lwrChart = depthChart[NflPosition.LWR.ToString()];
            Assert.NotEmpty(lwrChart);
            Assert.Equal(2, lwrChart.Count);
            Assert.Equal(lwrPlayer1.Name, lwrChart[0].Name);
            Assert.Equal(lwrPlayer1.Number, lwrChart[0].Number);
            Assert.Equal(lwrPlayer2.Name, lwrChart[1].Name);
            Assert.Equal(lwrPlayer2.Number, lwrChart[1].Number);
        }

        [Fact]
        public void RemovePlayerFromDepthChart_EmptyDepthChart_ReturnEmptyList()
        {
            var player1 = new Player { Name = "Tom Brady", Number = 12 };
            var mockRepository = new Mock<INflDepthChartRepository>();
            mockRepository.Setup(x => x.GetPositionDepthChart(It.IsAny<string>())).Returns(new List<Player>());
            var processor = new NflChartProcessor(mockRepository.Object);

            var depthChart = processor.RemovePlayerFromDepthChart(_position, player1);
            Assert.Empty(depthChart);
            mockRepository.Verify(x => x.RemovePlayer(It.IsAny<string>(), It.IsAny<Player>()), Times.Never);
        }

        [Fact]
        public void RemovePlayerFromDepthChart_PlayerDoesNotExistAtPosition_ReturnEmptyList()
        {
            var player1 = new Player { Name = "Tom Brady", Number = 12 };
            var mockRepository = new Mock<INflDepthChartRepository>();
            mockRepository.Setup(x => x.GetPositionDepthChart(It.IsAny<string>())).Returns(new List<Player>() { player1 });
            var processor = new NflChartProcessor(mockRepository.Object);

            var player2 = new Player { Name = "Blaine", Number = 11 };
            var depthChart = processor.RemovePlayerFromDepthChart(_position, player2);
            Assert.Empty(depthChart);
            mockRepository.Verify(x => x.RemovePlayer(It.IsAny<string>(), It.IsAny<Player>()), Times.Never);
        }

        [Fact]
        public void RemovePlayerFromDepthChart_PlayerRemoved_ReturnPlayerInList()
        {
            var player1 = new Player { Name = "Tom Brady", Number = 12 };
            var mockRepository = new Mock<INflDepthChartRepository>();
            mockRepository.Setup(x => x.GetPositionDepthChart(It.IsAny<string>())).Returns(new List<Player>() { player1 });
            var processor = new NflChartProcessor(mockRepository.Object);

            var depthChart = processor.RemovePlayerFromDepthChart(_position, player1);
            Assert.Single(depthChart);
            var removedPlayer = depthChart.FirstOrDefault();
            Assert.NotNull(removedPlayer);

            Assert.Equal(player1.Name, removedPlayer.Name);
            Assert.Equal(player1.Number, removedPlayer.Number);
            mockRepository.Verify(x => x.RemovePlayer(It.IsAny<string>(), It.IsAny<Player>()), Times.Once);
        }
    }
}
