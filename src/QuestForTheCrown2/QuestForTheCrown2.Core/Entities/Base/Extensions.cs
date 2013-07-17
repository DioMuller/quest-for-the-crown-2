using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Base
{
    static class Extensions
    {
        public static EntityRelativePosition CloserTo(this IEnumerable<Entity> entities, Entity relativeTo)
        {
            if (relativeTo == null)
                throw new ArgumentNullException("relativeTo");

            return (from e in entities
                    let position = new Vector2(e.CenterPosition.X - relativeTo.CenterPosition.X, e.CenterPosition.Y - relativeTo.CenterPosition.Y)
                    let distance = position.Length()
                    orderby distance
                    select new EntityRelativePosition
                     {
                         Entity = e,
                         RelativeTo = relativeTo,
                         Position = position,
                         Distance = distance
                     }).FirstOrDefault();
        }
    }
}
