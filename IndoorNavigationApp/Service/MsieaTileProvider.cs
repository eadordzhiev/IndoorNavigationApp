using System;
using IndoorNavigationApp.Models.Dto;

namespace IndoorNavigationApp.Service
{
    public class MsieaTileProvider : ITileProvider
    {
        public uint TileWidth { get; }

        public uint TileHeight { get; }

        public uint XCount { get; }

        public uint YCount { get; }

        private readonly string _uriTemplate;

        public MsieaTileProvider(MapDetailsDto mapDetailsDto)
        {
            TileWidth = TileHeight = 256;
            XCount = mapDetailsDto.TilesXcount;
            YCount = mapDetailsDto.TilesYcount;
            _uriTemplate = mapDetailsDto.TilesDir.Replace("x-y", "{0}-{1}");
        }

        public Uri GetTileUri(uint x, uint y)
        {
            if (x >= XCount)
            {
                throw new ArgumentOutOfRangeException(nameof(x));
            }
            if (y >= YCount)
            {
                throw new ArgumentOutOfRangeException(nameof(y));
            }

            return new Uri(string.Format(_uriTemplate, x, y));
        }
    }
}