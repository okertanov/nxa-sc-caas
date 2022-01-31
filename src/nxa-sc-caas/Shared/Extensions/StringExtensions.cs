using System;
using System.Text.RegularExpressions;

namespace NXA.SC.Caas.Extensions
{
    public static class StringExtensions
    {
        public static bool IsBase64String(this string base64)
        {
            Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
            return System.Convert.TryFromBase64String(base64, buffer , out int bytesParsed);
        }
        public static string PadBase64String(this string byteStr)
        {
            var byteStrLength = byteStr!.Length;
            var byteLenghtDivRest = byteStrLength % 4;
            byteStr = byteStr.PadRight(byteStrLength + byteLenghtDivRest, '=');
            return byteStr;
        }
        public static string GetSolContractName(this string contractSrc)
        {
            return Regex.Match(contractSrc, @"(?<=\bcontract\s)(\w+)").Value;
        }
    }
}
