using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Core.Settings.V1
{
    public class GameBoardSettings
    {
        public double GameBoardWidth { get; set; }
        public double GameBoardHeight { get; set; }

        public double GridSnapX { get; set; }
        public double GridSnapY { get; set; }

        public double Zoom { get; set; }

        public bool SelectNewlyCreatedBlock { get; set; }

        public ComponentSettings[] Components { get; set; }
        public FlagSemanticSettings[] FlagSemantics { get; set; }
        public LayerSettings LayerSettings { get; set; }

        public static GameBoardSettings GenerateDefault()
        {
            return new GameBoardSettings
            {
                GameBoardWidth = 400.0,
                GameBoardHeight = 2000.0,

                GridSnapX = 40.0,
                GridSnapY = 50.0,

                Zoom = 0.8,

                SelectNewlyCreatedBlock = true,

                Components = new[]
                {
                    new ComponentSettings { ShortToolTipText="CA", Type = "ComponentA" },
                    new ComponentSettings { Path = "..\\relative\\or\\absolute\\path", SearchPattern = "*Component.cs" },
                    new ComponentSettings
                    {
                        ShortToolTipText ="CA",
                        Type = "ComponentA",
                        Properties = new Property[]
                        {
                            new Property { Name = "MyBoolean", Type = "Boolean", Description = "This is to activate or deactivate" },
                            new Property { Name = "MyOtherBoolean", Type = "Bool" },
                            new Property { Name = "MyInt", Type = "int" },
                            new Property { Name = "MyOtherInteger", Type = "Integer" },
                            new Property { Name = "MyFloat", Type = "float" },
                            new Property
                            {
                                Name = "MyString",
                                Type = "String",
                                Values = new []
                                {
                                    "value 1",
                                    "value 2"
                                }
                            },
                            new Property { Name = "MyGuid", Type = "Guid" },
                        }
                    }
                },

                FlagSemantics = new[]
                {
                    new FlagSemanticSettings { FlagNumber = 0, Text = "Is Take Off" },
                },

                LayerSettings = new LayerSettings
                {
                    Default = 1,
                    Layers = new[]
                    {
                        new Layer { Number = 0, Name = "Background", Description = "Farthest layer" },
                        new Layer { Number = 1, Name = "Players" },
                        new Layer { Number = 2, Name = "Effects" },
                        new Layer { Number = 3, Name = "UI" },
                        new Layer { Number = 255, Name = "Foreground", Description = "Nearest layer" },
                    },
                }
            };
        }
    }
}
