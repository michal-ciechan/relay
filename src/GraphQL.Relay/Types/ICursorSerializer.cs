using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Panic.StringUtils;

namespace GraphQL.Relay.Types
{
    public interface ICursorSerializer
    {
        string Int64ToCursor(long? value);

        long? FromCursorToInt64(string? cursor);
    }

    public abstract class DelegateSerializer : ICursorSerializer
    {
        public string Int64ToCursor(long? value) => ToCursor(value);

        public long? FromCursorToInt64(string? cursor) => FromCursor<long>(cursor);

        public abstract string ToCursor<T>(T value);
        public abstract T FromCursor<T>(string value);
    }

    public class TextCursorSerializer : ICursorSerializer
    {
        private const string CursorPrefix = "cursor/";

        public string Int64ToCursor(long? value)
        {
            if (value == null)
                return default;
            
            return CursorPrefix + value.Value.ToString(CultureInfo.InvariantCulture);
        }

        long? ICursorSerializer.FromCursorToInt64(string? cursor)
        {
            if (string.IsNullOrEmpty(cursor))
                return default;

            if (!cursor.StartsWith(CursorPrefix, StringComparison.Ordinal))
                return FromBase64Cursor<long>(cursor);
            
            if (cursor.Length <= CursorPrefix.Length)
            {
                return FromBase64Cursor<long>(cursor);
            }

            var cursorIdSpan = cursor.AsSpan(CursorPrefix.Length);

#if NETSTANDARD2_1
            return long.TryParse(cursorIdSpan, out var result) ? result : FromBase64Cursor<long>(cursor);
#else
            return long.TryParse(cursorIdSpan.ToString(), out var result) ? result : FromBase64Cursor<long>(cursor);
#endif
        }

        private T FromBase64Cursor<T>(string cursor)
        {
            if (string.IsNullOrEmpty(cursor))
                return default;
            
            string decodedValue;
            try
            {
                decodedValue = Base64Decode(cursor);
            }
            catch (FormatException)
            {
                return default;
            }
            
            var prefixIndex = CursorPrefix.Length + 1;
            
            if (decodedValue.Length <= prefixIndex)
                return default;

            var value = decodedValue.AsSpan(CursorPrefix.Length);
            
            return (T)Convert.ChangeType(value.ToString(), typeof(T), CultureInfo.InvariantCulture);
        }
        
        
        private static string Base64Decode(string value) => Encoding.UTF8.GetString(Convert.FromBase64String(value));

        private static string Base64Encode(string value) => Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
    }
}