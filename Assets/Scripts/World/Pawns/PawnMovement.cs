using System.Collections.Generic;
using GoRogue;
using UnityEngine;

namespace World.Pawns
{
    public class PawnMovement
    {
        private Coord _nextPosition;
        
        private float _movementPercent;
        
        public System.Action<Direction> onChangeDirection;

        public Coord position;

        public Direction facing;

        public Queue<Coord> Path { get; }

        public Vector3 SpritePosition =>
            new(Mathf.Lerp(this.position.X, this._nextPosition.X, this._movementPercent),
                Mathf.Lerp(this.position.Y, this._nextPosition.Y, this._movementPercent), 0);
    }
}
