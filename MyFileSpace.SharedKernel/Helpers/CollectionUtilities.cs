using System.Text;

namespace MyFileSpace.SharedKernel.Helpers
{
    public static class CollectionUtilities
    {
        public static string ToString<T>(this IEnumerable<T> c)
        {
            StringBuilder sb = new StringBuilder("[");

            IEnumerator<T> e = c.GetEnumerator();

            if (e.MoveNext())
            {
                sb.Append(e.Current?.ToString());

                while (e.MoveNext())
                {
                    sb.Append(", ");
                    sb.Append(e.Current?.ToString());
                }
            }

            sb.Append(']');

            return sb.ToString();
        }
    }
}
