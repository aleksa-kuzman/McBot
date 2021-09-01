using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace McBot.Core
{
    public class EnumerationBase : IComparable
    {
        public string Name { get; private set; }
        public long Id { get; private set; }

        protected EnumerationBase(string name, long id)
        {
            Name = name;
            Id = id;
        }

        public override string ToString()
        {
            return Name;
        }

        public static IEnumerable<T> GetAll<T>() where T : EnumerationBase
        {
            return typeof(T).GetFields(
                BindingFlags.Public |
                BindingFlags.Static |
                BindingFlags.DeclaredOnly)
                .Select(m => m.GetValue(null))
                .Cast<T>();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(EnumerationBase))
            {
                return false;
            }

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = Id.Equals(((EnumerationBase)obj).Id);

            return typeMatches && valueMatches;
        }

        public int CompareTo(object obj)
        {
            return Id.CompareTo(((EnumerationBase)obj).Id);
        }
    }
}