using TradingSolutions.Application.Enums;
using TradingSolutions.Application.Models;

namespace TradingSolutions.Application.Requests.Nfl
{
    public class RemoveNflPlayerRequest
    {
        public NflPosition Position { get; set; }
        public Player Player { get; set; }
    }
}
