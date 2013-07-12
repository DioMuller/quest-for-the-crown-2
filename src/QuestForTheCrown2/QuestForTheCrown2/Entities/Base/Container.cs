using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Base
{
    public class Container
    {
        public Container()
        {
        }

        public Container(int quantity)
        {
            Quantity = quantity;
            Maximum = quantity;
        }

        public Container(int quantity, int? maximum)
        {
            Quantity = quantity;
            Maximum = maximum;
        }

        public int? Maximum { get; set; }
        public int Quantity { get; set; }

        public static Container operator --(Container a)
        {
            return a - 1;
        }

        public static Container operator ++(Container a)
        {
            return a + 1;
        }

        public static bool operator <=(Container a, int quantity)
        {
            if (a == null)
                a = new Container(0);

            return a.Quantity <= quantity;
        }

        public static bool operator >=(Container a, int quantity)
        {
            if (a == null)
                a = new Container(0);

            return a.Quantity >= quantity;
        }

        public static bool operator <(Container a, int quantity)
        {
            if (a == null)
                a = new Container(0);

            return a.Quantity < quantity;
        }

        public static bool operator >(Container a, int quantity)
        {
            if (a == null)
                a = new Container(0);
            return a.Quantity > quantity;
        }

        public static Container operator -(Container a, int quantity)
        {
            if (a.Maximum != null)
                return new Container(Math.Min(a.Quantity - quantity, a.Maximum.Value), a.Maximum);
            return new Container(a.Quantity + quantity, a.Maximum);
        }

        public static Container operator +(Container a, int quantity)
        {
            if (a.Maximum != null)
                return new Container(Math.Min(a.Quantity + quantity, a.Maximum.Value), a.Maximum);
            return new Container(a.Quantity + quantity, a.Maximum);
        }

        public static bool operator ==(Container a, int quantity)
        {
            return a.Quantity == quantity;
        }

        public static bool operator !=(Container a, int quantity)
        {
            return a.Quantity != quantity;
        }

        public override bool Equals(object obj)
        {
            return this == obj as Container;
        }

        public override int GetHashCode()
        {
            return Quantity;
        }

        public static implicit operator int(Container container)
        {
            return container.Quantity;
        }

        public static implicit operator int?(Container container)
        {
            if (container == null)
                return null;
            return container.Quantity;
        }
    }
}
