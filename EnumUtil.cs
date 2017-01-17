using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities
{
    public static class EnumUtil
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }


        public static T Random<T>(Random r)
            where T : struct
        {
            Array values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(r.Next(values.Length));
        }

        public static string GetDescription<T>(this T enumerationValue)
            where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();
        }
    }

    public class EnumParser<T>
        where T : struct
    {
        private Dictionary<String, T> _Dict = new Dictionary<string, T>();

        public EnumParser()
        {
            foreach (var e in EnumUtil.GetValues<T>())
            {
                _Dict[e.GetDescription()] = e;
            }
        }

        public T this[String inp]
        {
            get
            {
                lock (this._Dict)
                    return this._Dict[inp];
            }
        }

        public T TryParse(String inp, T default_return)
        {
            lock (this._Dict)
            {
                T o;
                if (inp == null) return default_return;
                else if (this._Dict.TryGetValue(inp, out o)) return o;
                else return default_return;
            }
        }
    }

    /// <summary>
    /// The base classes must be Enums!
    /// </summary>
    /// <typeparam name="Enum1"></typeparam>
    /// <typeparam name="Enum2"></typeparam>
    public class TwoEnums<Enum1, Enum2>
        where Enum1 : struct
        where Enum2 : struct
    {
        public static bool Equals(TwoEnums<Enum1, Enum2> a, TwoEnums<Enum1, Enum2> b)
        {
            return (a.e1.Equals(b.e1)) && (a.e2.Equals(b.e2));
        }

        public readonly Enum1 e1;
        public readonly Enum2 e2;

        public TwoEnums(Enum1 col, Enum2 st)
        {
            if (!typeof(Enum1).IsEnum)
                throw new ArgumentException("Enum1 must be of Enum type", "Enum1");

            if (!typeof(Enum2).IsEnum)
                throw new ArgumentException("Enum1 must be of Enum type", "Enum2");

            this.e1 = col;
            this.e2 = st;
        }

        public override string ToString()
        {
            return EnumUtil.GetDescription(this.e1) + " " + EnumUtil.GetDescription(this.e2);
        }

        public override int GetHashCode()
        {
            return this.e2.GetHashCode() * 17 + this.e1.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is TwoEnums<Enum1, Enum2>)
            {
                var obj_s = obj as TwoEnums<Enum1, Enum2>;
                return TwoEnums<Enum1, Enum2>.Equals(this, obj_s);
            }
            else return false;
        }

        public static TwoEnums<Enum1, Enum2>[] Random(Random r, int count)
        {
            var ls = new List<TwoEnums<Enum1, Enum2>>();

            while (ls.Count < count)
            {
                var new_symbol = new TwoEnums<Enum1, Enum2>(EnumUtil.Random<Enum1>(r), EnumUtil.Random<Enum2>(r));

                foreach (var old_symbol in ls)
                {
                    if (new_symbol.Equals(old_symbol))
                    {
                        new_symbol = null;
                        break;
                    }
                }

                if (new_symbol != null) ls.Add(new_symbol);
            }

            return ls.ToArray();
        }
    }
}
