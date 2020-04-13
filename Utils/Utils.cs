using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;
using System.IO;

namespace CustomSpawns.Utils
{
    public static class Utils
    {

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

        private static string[] dependentModsArray;

        public static string[] GetAllValidDependentModsPaths()
        {
            if (dependentModsArray == null)
            {
                List<string> validPaths = new List<string>();
                //construct the array
                string basePath = Path.Combine(BasePath.Name, "Modules");
                string[] all = Directory.GetFiles(basePath);
                foreach(string path in all)
                {
                    if(File.Exists(Path.Combine(path, "CustomSpawns")))
                    {
                        validPaths.Add(Path.Combine(path, "CustomSpawns"));
                    }
                }
                dependentModsArray = validPaths.ToArray();
            }
            return dependentModsArray;
        }

    }
}
