using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SandSim
{
    internal class World
    {
        private Game1 _game1;
        private int tileSize = 10;
        Rectangle _updateArea = new Rectangle(0, 0, 100, 100);
        Rectangle _renderedArea = new Rectangle(0, 0, 100, 100);
        private Dictionary<(int x, int y), Tile> tiles = new Dictionary<(int x, int y), Tile>();
        private int spawnMode;
        private bool waterFlowLeftPriority = true;
        public World(Game1 game1)
        {
            _game1 = game1;
            _renderedArea.Width = game1.GraphicsDevice.Viewport.Width / tileSize;
            _renderedArea.Height = game1.GraphicsDevice.Viewport.Height / tileSize;
            _updateArea.Width = game1.GraphicsDevice.Viewport.Width / tileSize;
            _updateArea.Height = game1.GraphicsDevice.Viewport.Height / tileSize;

            for (int x = _renderedArea.Left - 1; x < _renderedArea.Right + 1; x++)
            {
                tiles.Add((x, -1), new BlockTile());
                tiles.Add((x, _renderedArea.Bottom), new BlockTile());
            }
            for (int y = _renderedArea.Top; y < _renderedArea.Bottom; y++)
            {
                tiles.Add((-1, y), new BlockTile());
                tiles.Add((_renderedArea.Right, y), new BlockTile());
            }
        }
        public void Update(GameTime gameTime)
        {
            UpdateTiles(gameTime);
            if (Input.Actions.Contains("BlockMode"))
            {
                spawnMode = 0;
            }
            if (Input.Actions.Contains("SandMode"))
            {
                spawnMode = 1;
            }
            if (Input.Actions.Contains("leftClick"))
            {
                var mousePosition = Input.GetMousePosition();
                var coord = (mousePosition.x / 10, _renderedArea.Height - mousePosition.y / 10 - 1);
                switch (spawnMode)
                {
                    case 0:
                        tiles.TryAdd(coord, new BlockTile());
                        break;
                    case 1:
                        tiles.TryAdd(coord, new SandTile());
                        break;
                }
            }
            if (Input.Actions.Contains("rightClick"))
            {
                var mousePosition = Input.GetMousePosition();
                var coord = (mousePosition.x / 10, _renderedArea.Height - mousePosition.y / 10 - 1);
                tiles.TryAdd(coord, new WaterTile());
            }
        }
        public void UpdateTiles(GameTime gameTime)
        {
            for (int x = _updateArea.Left; x < _updateArea.Right; x++)
            {
                for (int y = _updateArea.Top; y < _updateArea.Bottom; y++)
                {
                    if (!tiles.TryGetValue((x, y), out Tile tile))
                        continue;
                    if (tile is SandTile)
                    {
                        (int x, int y) coordBelow = (x, y - 1);
                        (int x, int y) coordLeft = (x - 1, y - 1);
                        (int x, int y) coordRight = (x + 1, y - 1);
                        if (!tiles.TryGetValue(coordBelow, out Tile tileBelow))
                        {
                            tiles.Add(coordBelow, tile);
                            tiles.Remove((x, y));
                        }

                        if (!(tileBelow is SandTile))
                            continue;

                        if (!tiles.TryGetValue(coordLeft, out _))
                        {
                            tiles.Add(coordLeft, tile);
                            tiles.Remove((x, y));
                        }
                        else if (!tiles.TryGetValue(coordRight, out _))
                        {
                            tiles.Add(coordRight, tile);
                            tiles.Remove((x, y));
                        }
                    }
                    else if (tile is WaterTile)
                    {
                        UpdateWaterTile(x, y);
                        //if (!tiles.TryGetValue(coordBelow, out _))
                        //{
                        //    tiles.Add(coordBelow, tile);
                        //    tiles.Remove((x, y));
                        //    continue;
                        //}
                        //int i = 1;
                        //bool leftOpen = !tiles.TryGetValue((x - 1, y), out _);
                        //bool rightOpen = !tiles.TryGetValue((x + 1, y), out _);
                        //while (x - i >= _updateArea.Left && x + i < _updateArea.Right)
                        //{
                        //    bool oldLeftOpen = leftOpen;
                        //    bool oldRightOpen = rightOpen;
                        //    leftOpen = leftOpen && tiles.TryGetValue((x - i, y), out _);
                        //    rightOpen = rightOpen && tiles.TryGetValue((x + i, y), out _);
                        //    if (!leftOpen && !rightOpen)
                        //    {
                        //        if (oldLeftOpen)
                        //        {
                        //            tiles.Add((x - 1, y), tile);
                        //            tiles.Remove((x, y));
                        //        }
                        //        else if (oldRightOpen)
                        //        {
                        //            tiles.Add((x + 1, y), tile);
                        //            tiles.Remove((x, y));
                        //        }
                        //        break;

                        //    }
                        //    if (leftOpen)
                        //    {
                        //        if (!tiles.TryGetValue((x - i, y - 1), out Tile t))
                        //        {
                        //            tiles.Add((x - 1, y), tile);
                        //            tiles.Remove((x, y));
                        //        }
                        //        else if (!(t is WaterTile))
                        //        {
                        //            leftOpen = false;
                        //        }

                        //    }
                        //    else if (rightOpen)
                        //    {
                        //        if (!tiles.TryGetValue((x + i, y - 1), out Tile t))
                        //        {
                        //            tiles.Add((x + 1, y), tile);
                        //            tiles.Remove((x, y));
                        //        }
                        //        else if (!(t is WaterTile))
                        //        {
                        //            rightOpen = false;
                        //        }
                        //    }
                        //    i++;
                        //}
                    }
                }
            }
        }
        void UpdateWaterTile(int x, int y)
        {
            Tile tile = tiles[(x, y)];
            // below empty – fall one
            // below not water – do nothing
            // below one water – if pair length go left else go right
            //                   if left and right filled – go to mid

            //(int x, int y) coordBelow = (x, y - 1);
            if (IsEmpty(x, y - 1))
            {
                tiles.Add((x, y - 1), tile);
                tiles.Remove((x, y));
                return;
            }
            else if (IsTileType<WaterTile>(x, y - 1))
            {
                int waterTilesToLeft = 0;
                while (true)
                {
                    if (IsTileType<WaterTile>(x - waterTilesToLeft - 1, y - 1))
                        waterTilesToLeft++;
                    else
                    {
                        break;
                    }
                }
                int waterTilesToRight = 0;
                while (true)
                {
                    if (IsTileType<WaterTile>(x + waterTilesToRight + 1, y - 1))
                        waterTilesToRight++;
                    else
                    {
                        break;
                    }
                }
                bool isEmptyToLeft = IsEmpty(x - waterTilesToLeft - 1, y - 1);
                bool isEmptyToRight = IsEmpty(x + waterTilesToRight + 1, y - 1);
                if (isEmptyToLeft || isEmptyToRight)
                {
                    bool putWaterLeftSide = (waterTilesToLeft + waterTilesToRight) % 2 == 0;
                    //bool putWaterLeftSide = waterFlowLeftPriority;
                    //waterFlowLeftPriority = !waterFlowLeftPriority;
                    if (isEmptyToLeft && isEmptyToRight)
                    {
                        bool isWaterUnderLeftSide = IsTileType<WaterTile>(x - waterTilesToLeft - 1, y - 2);
                        bool isWaterUnderRightSide = IsTileType<WaterTile>(x + waterTilesToRight + 1, y - 2);
                        if (isWaterUnderLeftSide && !isWaterUnderRightSide)
                        {
                            putWaterLeftSide = true;
                        }
                        else if (!isWaterUnderLeftSide && isWaterUnderRightSide)
                        {
                            putWaterLeftSide = false;
                        }
                    }
                    else
                    {
                        if (!isEmptyToLeft)
                            putWaterLeftSide = false;
                        if (!isEmptyToRight)
                            putWaterLeftSide = true;
                    }
                    if (putWaterLeftSide)
                    {
                        tiles.Add((x - waterTilesToLeft - 1, y - 1), tile);
                        tiles.Remove((x, y));
                        return;
                    }
                    else
                    {
                        tiles.Add((x + waterTilesToRight + 1, y - 1), tile);
                        tiles.Remove((x, y));
                        return;
                    }
                }
                else
                {
                    int middle = x + (waterTilesToRight - waterTilesToLeft) / 2;
                    if (IsEmpty(middle, y))
                    {
                        tiles.Add((middle, y), tile);
                        tiles.Remove((x, y));
                        return;
                    }
                    bool leftBlocked = false;
                    bool rightBlocked = false;
                    int i = 1;
                    while (!leftBlocked || !rightBlocked)
                    {
                        bool leftIsEmpty = IsEmpty(middle + i, y);
                        if (!leftIsEmpty && !IsTileType<WaterTile>(middle + i, y))
                            leftBlocked = true;
                        if (leftIsEmpty && !leftBlocked)
                        {
                            tiles.Add((middle + i, y), tile);
                            tiles.Remove((x, y));
                            break;
                        }
                        bool rightIsEmpty = IsEmpty(middle - i, y);
                        if (!rightIsEmpty && !IsTileType<WaterTile>(middle - i, y))
                            rightBlocked = true;
                        if (rightIsEmpty && !rightBlocked)
                        {
                            tiles.Add((middle - i, y), tile);
                            tiles.Remove((x, y));
                            break;
                        }
                        i++;
                    }
                    return;
                }

            }
        }
        bool IsTileType<T>(int x, int y)
        {
            return tiles.TryGetValue((x, y), out Tile tile) && tile is T;
        }
        bool IsEmpty(int x, int y)
        {
            return !tiles.ContainsKey((x, y));
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int x = _renderedArea.Left; x < _renderedArea.Right; x++)
            {
                for (int y = _renderedArea.Top; y < _renderedArea.Bottom; y++)
                {
                    if (!tiles.TryGetValue((x, y), out Tile tile))
                        continue;

                    spriteBatch.Draw(_game1.Textures["default"], new Rectangle(x * tileSize, (_renderedArea.Height - 1 - y) * tileSize, 10, 10), tile.Color);
                }
            }
        }
    }
}
