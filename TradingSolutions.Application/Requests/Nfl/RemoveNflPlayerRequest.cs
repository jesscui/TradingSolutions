using FluentValidation;
using TradingSolutions.Application.Enums;
using TradingSolutions.Application.Models;

namespace TradingSolutions.Application.Requests.Nfl
{
    public class RemoveNflPlayerRequest
    {
        public string Position { get; set; }
        public Player Player { get; set; }
    }

    public class RemoveNflPlayerRequestValidator : AbstractValidator<RemoveNflPlayerRequest>
    {
        public RemoveNflPlayerRequestValidator()
        {
            RuleFor(x => x.Player).SetValidator(new PlayerValidator());
            RuleFor(x => x.Position).IsEnumName(typeof(NflPosition));
        }
    }
}
