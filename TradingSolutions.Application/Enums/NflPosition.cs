using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TradingSolutions.Application.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum NflPosition
    {
        LWR,
        RWR,
        LT,
        LG,
        C,
        RG,
        RT,
        TE,
        QB,
        RB
    }
}
