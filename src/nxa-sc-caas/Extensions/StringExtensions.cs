using System;

namespace NXA.SC.Caas.Extensions {
    public static class StringExtensions {
        public static bool IsBase64String(this string base64)
        {
            Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
            return System.Convert.TryFromBase64String(base64, buffer , out int bytesParsed);
        }
    }
}
