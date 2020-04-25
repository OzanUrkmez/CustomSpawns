using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;
using System.IO;
using System.Xml;

namespace CustomSpawns.Utils
{
    public static class Utils
    {
        public static bool IsCustomSpawnsStringID(string strindID)
        {
            return (strindID.StartsWith("cs_"));
        }
        public static IEnumerable<T> TakeAllButLast<T>(this IEnumerable<T> source)
        {
            var it = source.GetEnumerator();
            bool hasRemainingItems = false;
            bool isFirst = true;
            T item = default(T);

            do
            {
                hasRemainingItems = it.MoveNext();
                if (hasRemainingItems)
                {
                    if (!isFirst) yield return item;
                    item = it.Current;
                    isFirst = false;
                }
            } while (hasRemainingItems);
        }

        public static int GetTotalPrisonerCounts(List<PrisonerInfo> prisonerInfos)
        {
            int returned = 0;
            foreach(var p in prisonerInfos)
            {
                returned += p.count;
            }
            return returned;
        }

    }
}
