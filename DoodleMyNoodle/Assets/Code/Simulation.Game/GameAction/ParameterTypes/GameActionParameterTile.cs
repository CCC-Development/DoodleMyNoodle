﻿using System;
using Unity.Entities;
using Unity.Mathematics;

public class GameActionParameterTile
{
    public class Description : GameAction.ParameterDescription
    {
        // NB: default parameters should match to 'all tiles'

        public int RangeFromInstigator { get; private set; }
        public TileFlags TileFilter = TileFlags.All;
        public bool IncludeSelf = true;
        public bool MustBeReachable = false;
        public bool RequiresAttackableEntity = false;

        // If used, only tiles with a matching tile actor will be accepted
        public delegate bool TileActorPredicate(Entity tileActor, ISimWorldReadAccessor accessor);
        public TileActorPredicate CustomTileActorPredicate = null;

        // If used, only matching tiles will be accepted
        public delegate bool TilePredicate(int2 tile, Entity tileEntity, ISimWorldReadAccessor accessor);
        public TilePredicate CustomTilePredicate = null;

        public Description(int rangeFromInstigator)
        {
            if (rangeFromInstigator > Pathfinding.MAX_PATH_LENGTH)
                rangeFromInstigator = Pathfinding.MAX_PATH_LENGTH;

            RangeFromInstigator = rangeFromInstigator;
        }
    }

    [NetSerializable]
    public class Data : GameAction.ParameterData
    {
        public int2 Tile;

        public Data() { }

        public Data(int2 tile)
        {
            Tile = tile;
        }
    }
}