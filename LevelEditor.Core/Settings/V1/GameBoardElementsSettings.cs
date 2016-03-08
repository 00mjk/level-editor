using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayerDataReaderWriter;

namespace LevelEditor.Core.Settings.V1
{
    public class GameBoardElementsSettings
    {
        public LayerBlockElementDefinition[] Elements { get; set; }

        public static GameBoardElementsSettings GenerateDefault()
        {
            return new GameBoardElementsSettings
            {
                Elements = new[]
                {
                    // boosts
                    LayerBlockElementDefinition.GenerateDefault("All;Boost", 0, "../../../Icons/boost_lvl01.png"),
                    LayerBlockElementDefinition.GenerateDefault("All;Boost", 1, "../../../Icons/boost_lvl02.png"),
                    LayerBlockElementDefinition.GenerateDefault("All;Boost", 2, "../../../Icons/boost_lvl03.png"),

                    // gems/coins
                    LayerBlockElementDefinition.GenerateDefault("All;Gems", 21, "../../../../showcaseapp1/Assets/Textures/Common/Gems/gemBlue.png", 0.5, 0.5),
                    LayerBlockElementDefinition.GenerateDefault("All;Gems", 22, "../../../../showcaseapp1/Assets/Textures/Common/Gems/gemPurple.png", 0.5, 0.5),
                    LayerBlockElementDefinition.GenerateDefault("All;Gems", 22, "../../../../showcaseapp1/Assets/Textures/Common/Gems/gemRed.png", 0.5, 0.5),

                    // Obstacles
                    LayerBlockElementDefinition.GenerateDefault("All;Obstacles", 41, "../../../../showcaseapp1/Assets/Textures/Dungeon/LayerA/DungeonA00.png"),
                    LayerBlockElementDefinition.GenerateDefault("All;Obstacles", 40, "../../../../showcaseapp1/Assets/Textures/Dungeon/LayerA/DungeonA01.png"),

                    // Power Ups
                    LayerBlockElementDefinition.GenerateDefault("All;Powerups", 100, "../../../../showcaseapp1/Assets/Textures/Common/PowerUps/bulletTime.png", 0.6, 0.6),
                    LayerBlockElementDefinition.GenerateDefault("All;Powerups", 101, "../../../../showcaseapp1/Assets/Textures/Common/PowerUps/shield.png", 0.8, 0.8),

                    // A.I.
                    LayerBlockElementDefinition.GenerateDefault("All;AI", 202, "../../../Icons/waypoint_01.png"),
                    LayerBlockElementDefinition.GenerateDefault("All;AI", 203, "../../../Icons/waypoint_02.png"),
                    LayerBlockElementDefinition.GenerateDefault("All;AI", 204, "../../../Icons/waypoint_03.png"),
                }
            };
        }
    }
}
