using Microsoft.Xna.Framework;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Levels.Mapping;
using System;

namespace QuestForTheCrown2.Entities.Behaviors
{
    class FollowBehavior : WalkBehavior
    {
        public float AcceptableDistanceError { get; set; }
        public float Distance { get; set; }
        public Entity Following { get; set; }

        public FollowBehavior()
        {
            Group = "movement";
            Distance = 32 * 2;
            AcceptableDistanceError = 5;
        }

        public override bool Active
        {
            get { return Following != null; }
        }

        public override void Update(GameTime gameTime, Map map)
        {
            var direction = new Vector2(
                Following.Position.X - Entity.Position.X,
                Following.Position.Y - Entity.Position.Y);

            var route = direction;
            var length = route.Length();

            if (length < Distance - AcceptableDistanceError)
                route = Vector2.Zero;
            else if (route.Length() > 1)
                route.Normalize();

            Walk(gameTime, map, route);
        }
    }
}
