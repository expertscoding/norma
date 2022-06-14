using EC.Norma.Json.Entities;
using EC.Norma.Json.Providers;
using EC.Norma.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace EC.Norma.Json
{
    public static class NormaJsonExtensions
    {

        public static INormaBuilder AddNormaJsonStore(this INormaBuilder builder, IEnumerable<Profile> profiles)
        {
            builder.Services.AddSingleton(sp => new NormaContext(profiles));

            builder.Services.AddSingleton<INormaProvider, JsonNormaProvider>();

            return builder;
        }
    }
}
