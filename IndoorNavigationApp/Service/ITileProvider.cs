using System;

namespace IndoorNavigationApp.Service
{
    public interface ITileProvider
    {
        uint TileWidth { get; }

        uint TileHeight { get; }

        uint XCount { get; }

        uint YCount { get; }

        Uri GetTileUri(uint x, uint y);
    }
}