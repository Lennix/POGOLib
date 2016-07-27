using POGOProtos.Enums;
using POGOProtos.Networking.Responses;
using POGOProtos.Settings.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POGOLib.Pokemon
{
    static class Templates
    {
        public static GetAssetDigestResponse AssetDigest { get; set; }

        public static DownloadItemTemplatesResponse ItemTemplates { get; set; }

        public static IEnumerable<PokemonSettings> GetPokemonSettings()
        {
            return
                ItemTemplates.ItemTemplates.Select(i => i.PokemonSettings)
                    .Where(p => p != null && p?.FamilyId != PokemonFamilyId.FamilyUnset);
        }
    }
}
