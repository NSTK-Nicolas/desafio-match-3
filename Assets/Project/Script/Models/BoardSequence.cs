﻿using System.Collections.Generic;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Project.Script.Models
{
    public class BoardSequence
    {
        public List<MovedTileInfo> MovedTiles { get; set; }
        public List<AddedTileInfo> AddedTiles { get; set; }
        public List<Vector2Int> MatchedPosition { get; set; }
    }
}
