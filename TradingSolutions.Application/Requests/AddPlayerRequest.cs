using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingSolutions.Application.Enums;
using TradingSolutions.Application.Models;

namespace TradingSolutions.Application.Requests
{
    public class AddPlayerRequest
    {
        public NhlPositions Position { get; set; }
        public Player Player { get; set; }
        public int PositionDepth { get; set; }
    }
}
