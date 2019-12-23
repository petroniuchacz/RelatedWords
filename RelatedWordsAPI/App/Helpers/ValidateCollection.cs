using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RelatedWordsAPI.App.Helpers
{
    public static class ValidateCollection
    {
        public static bool Validate<T> (IEnumerable<T> originalCol, IEnumerable<T> changedCol, Func<T, IEnumerable<T>, bool> evaluate)
        {
            foreach (T item in changedCol)
            {
                bool valid = evaluate(item, originalCol);
                if (!valid)
                    return false;
            }
            return true;
        }

    }
}
