using TradingSolutions.Application.Enums;
using TradingSolutions.Application.Models;

namespace TradingSolutions.Application.Requests.Nfl
{
    public class AddNflPlayerRequest
    {
        public NflPosition Position { get; set; }
        public Player Player { get; set; }
        public int PositionDepth { get; set; } = -1;
    }
}
