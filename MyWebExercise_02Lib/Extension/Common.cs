using System.Globalization;
using System.Text.Json;

namespace JackToolLib.Extension
{
    static class Common
    {

        /// <summary>
        /// Deeps the clone via json.
        /// </summary>
        /// <typeparam name="T">Target class</typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static T DeepCloneViaJson<T>(this T source)
        {

            if (source != null)
            {
                //var serializedObj = JSON.Serialize(source);
                //return JSON.Deserialize<T>(serializedObj);
                var serializedObj = JsonSerializer.Serialize(source);
                return JsonSerializer.Deserialize<T>(serializedObj);
            }
            else
            { return default(T); }

        }


        /// <summary>
        /// To the title case.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static string ToTitleCase(this string input)
        {
            var txtInfo = new CultureInfo("en-US", false).TextInfo;
            return txtInfo.ToTitleCase(input.ToLower());
        }
    }
}
