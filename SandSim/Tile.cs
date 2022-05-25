using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SandSim
{
    internal abstract class Tile
    {
        public Color Color { get; set; }
    }
    internal class BlockTile : Tile
    {
        public BlockTile()
        {
            Color = Color.Gray;
        }
    }
    internal class SandTile : Tile
    {
        public SandTile()
        {
            Color = Color.Yellow;
        }
    }
    internal class WaterTile : Tile
    {
        public WaterTile()
        {
            Color = Color.Blue;
        }
    }
}
