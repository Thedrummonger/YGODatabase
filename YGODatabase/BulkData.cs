using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YGODatabase
{
    internal class BulkData
    {
        public static List<string> Conditions = new List<string>()
        {
            "Near Mint to Mint",
            "Lightly Played",
            "Moderately Played",
            "Heavy Played",
            "Damaged"
        };
        public static List<string> Rarities = new List<string>()
        {
            "Common",
            "Rare",
            "Starfoil Rare",
            "Shatterfoil Rare",
            "Super Rare",
            "Super Short Print",
            "Ultra Rare",
            "Ultra Rare (Pharaoh's Rare)",
            "Secret Rare",
            "Prismatic Secret Rare",
            "Quarter Century Secret Rare",
            "Collector's Rare",
            "Ultimate Rare",
            "Ghost Rare",
            "Starlight Rare",
            "10000 Secret Rare",

            "Gold Rare",
            "Premium Gold Rare",
            "Gold Secret Rare",
            "Ghost/Gold Rare",

            "Mosaic Rare",

            "Platinum Rare",
            "Platinum Secret Rare",

            "Duel Terminal Normal Parallel Rare",
            "Duel Terminal Rare Parallel Rare",
            "Duel Terminal Super Parallel Rare",
            "Duel Terminal Ultra Parallel Rare",

            "Normal Parallel Rare",
            "Super Parallel Rare",
            "Ultra Parallel Rare",

            "Short Print",
        };

        public static List<string> CardTypes = new List<string> {
          "Spell Card",
          "Effect Monster",
          "Normal Monster",
          "Flip Effect Monster",
          "Trap Card",
          "Union Effect Monster",
          "Fusion Monster",
          "Pendulum Effect Monster",
          "Link Monster",
          "XYZ Monster",
          "Synchro Monster",
          "Synchro Tuner Monster",
          "Tuner Monster",
          "Gemini Monster",
          "Normal Tuner Monster",
          "Spirit Monster",
          "Ritual Effect Monster",
          "Skill Card",
          "Token",
          "Pendulum Effect Fusion Monster",
          "Ritual Monster",
          "Toon Monster",
          "Pendulum Normal Monster",
          "Synchro Pendulum Effect Monster",
          "Pendulum Tuner Effect Monster",
          "XYZ Pendulum Effect Monster",
          "Pendulum Effect Ritual Monster",
          "Pendulum Flip Effect Monster"
        };
    }
}
