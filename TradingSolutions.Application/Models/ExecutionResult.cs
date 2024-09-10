using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingSolutions.Application.Models
{
    public class ExecutionResult
    {
        public bool IsValid { get; set; }
        public bool IsSuccess { get; set; }
        public bool IsError { get; set; }
        public string[] ErrorDetails { get; set; }
    }
}
