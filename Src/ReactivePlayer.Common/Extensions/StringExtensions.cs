using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Common.Extensions
{
    /// <summary>
    /// Provides additional custom methods for the <see cref="string"/> type.
    /// </summary>
    public static class StringExtensions
    {
        #region fields & constants

        [Obsolete]
        public const string Digits = "0123456789";
        [Obsolete]
        public const string LowerAphabet = "abcdefghijklmnopqrstuvwxyz";
        [Obsolete]
        public const string UpperAphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        [Obsolete]
        public const string Alphabet = StringExtensions.LowerAphabet + StringExtensions.UpperAphabet;
        [Obsolete]
        public const string LowerAccentedChars = "àçèéìòù";
        [Obsolete]
        public const string UpperAccentedChars = "ÀÇÈÉÌÒÙ";
        [Obsolete]
        public const string AccentedChars = StringExtensions.LowerAccentedChars + StringExtensions.UpperAccentedChars;

        #endregion

        /// <summary>
        /// Generates a random string using the <see cref="char"/>acters contained in the source <see cref="string"/>.
        /// </summary>
        /// <param name="sourceChars">The <see cref="string"/> or <see cref="IEnumerable{char}"/> that provides the <see cref="char"/>aters to use when composing the random <see cref="string"/>.</param>
        /// <param name="length">The length of the random <see cref="string"/>.</param>
        /// <param name="random">The <see cref="Random"/> to use when randomizing. When this function is used many times in a short period, it is reccomended to use the same instance of <see cref="Random"/> in order to ensure <see cref="string"/>s diversity.</param>
        /// <returns></returns>
        public static string Randomize(this IEnumerable<char> sourceChars, int length, Random random = null)
        {
            if (sourceChars == null)
                throw new ArgumentNullException(nameof(sourceChars));

            if (random == null)
                random = new Random(Environment.TickCount);

            var sb = new StringBuilder(length);
            var sourceCharsCount = (sourceChars as ICollection<char> ?? sourceChars.ToList()).Count;

            while (length-- > 0)
            {
                sb.Append(sourceChars.ElementAt(random.Next(0, sourceCharsCount)));
            }

            return sb.ToString();

            //return new string(Enumerable.Repeat(sourceChars, length).Select(chars => chars.ElementAt(random.Next(chars.Count()))).ToArray());
        }

        /// <summary>
        /// If the first <see cref="char"/> of the <see cref="string"/> is a letter, converts it to upper case.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="firstLetterOnly"></param>
        /// <returns></returns>
        public static string ToSentenceCase(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)
                || !char.IsLetter(value.FirstOrDefault()))
                return value;

            var valueChars = value.ToArray();
            valueChars[0] = char.ToUpper(valueChars[0]);

            return new string(valueChars);
        }

        /// <summary>
        /// Returns a value indicating whether a specified sub<see cref="string"/> occurs within this <see cref="string"/>.
        /// </summary>
        /// <param name="source">The <see cref="string"/> where to look for the value.</param>
        /// <param name="value">The <see cref="string"/> value to look for.</param>
        /// <param name="comparison">The <see cref="StringComparison"/> to use.</param>
        /// <returns>true if the value parameter occurs within this string, or if value is the empty string (""); otherwise, false.</returns>
        public static bool Contains(this string source, string value, StringComparison comparison = StringComparison.CurrentCulture)
        {
            return source.IndexOf(value, comparison) >= 0;
        }

        public static string RemoveDiacritics(this string s)
        {
            // TODO get more info's about NormalizationForm.*
            string normalizedString = s.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < normalizedString.Length; i++)
            {
                Char c = normalizedString[i];
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Returns the <paramref name="source"/> <see cref="string">string</see> if it's neither <see cref="null"/> or <see cref="string.Empty"/>, otherwise returns the <paramref name="alternative"/> <see cref="string"/>.
        /// </summary>
        /// <param name="source">The <see cref="string"/> to validate.</param>
        /// <param name="alternative">The <see cref="string"/> to return if the <paramref name="source"/> is <see cref="null"/> or <see cref="string.Empty"/>.</param>
        /// <returns></returns>
        public static string OrIfNullOrEmpty(this string source, string alternative)
        {
            return !string.IsNullOrEmpty(source) ? source : alternative;
        }
    }
}