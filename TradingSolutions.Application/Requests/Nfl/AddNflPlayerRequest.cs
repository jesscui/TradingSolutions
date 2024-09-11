using FluentValidation;
using TradingSolutions.Application.Enums;
using TradingSolutions.Application.Models;

namespace TradingSolutions.Application.Requests.Nfl
{
    public class AddNflPlayerRequest
    {
        public string Position { get; set; }
        public Player Player { get; set; }
        public int PositionDepth { get; set; } = -1;
    }

    public class AddNflPlayerRequestValidator : AbstractValidator<AddNflPlayerRequest>
    {
        public AddNflPlayerRequestValidator()
        {
            RuleFor(x => x.Player).SetValidator(new PlayerValidator());
            RuleFor(x => x.Position).IsEnumName(typeof(NflPosition)).WithMessage("Invalid Position");
        }
    }
}
