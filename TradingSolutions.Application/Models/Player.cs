using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingSolutions.Application.Models
{
    public class Player
    {
        public int Number { get; set; }
        public string Name { get; set; }
    }

    public class PlayerValidator : AbstractValidator<Player>
    {
        public PlayerValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Number).GreaterThanOrEqualTo(0);
        }
    }
}
