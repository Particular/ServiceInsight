﻿using System;

namespace Particular.ServiceInsight.Desktop.MessageList
{
    public class FileSizeFormatProvider : IFormatProvider, ICustomFormatter
    {
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter)) return this;
            return null;
        }

        private const string FileSizeFormat = "fs";
        private const Decimal OneKiloByte = 1024M;
        private const Decimal OneMegaByte = OneKiloByte * 1024M;
        private const Decimal OneGigaByte = OneMegaByte * 1024M;

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (format == null || !format.StartsWith(FileSizeFormat))
                return DefaultFormat(format, arg, formatProvider);
            
            if (arg is string)
                return DefaultFormat(format, arg, formatProvider);
            
            Decimal size;

            try
            {
                size = Convert.ToDecimal(arg);
            }
            catch (InvalidCastException)
            {
                return DefaultFormat(format, arg, formatProvider);
            }

            string suffix;
            if (size > OneGigaByte)
            {
                size /= OneGigaByte;
                suffix = "GB";
            }
            else if (size > OneMegaByte)
            {
                size /= OneMegaByte;
                suffix = "MB";
            }
            else if (size > OneKiloByte)
            {
                size /= OneKiloByte;
                suffix = "KB";
            }
            else
            {
                suffix = " B";
            }

            var precision = format.Substring(2);
            if (string.IsNullOrEmpty(precision)) precision = "2";
            return string.Format("{0:N" + precision + "} {1}", size, suffix);
        }

        private static string DefaultFormat(string format, object arg, IFormatProvider formatProvider)
        {
            var formattableArg = arg as IFormattable;
            if (formattableArg != null)
            {
                return formattableArg.ToString(format, formatProvider);
            }

            return arg.ToString();
        }
    }
}