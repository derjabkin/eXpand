using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo.Metadata;

namespace Xpand.Xpo.Converters.ValueConverters
{
    /// <summary>
    /// Summary description for DictionaryValueConverter.
    /// </summary>
    public class DictionaryValueConverter : ValueConverter
    {
        private const string keyDelimeter = "ﮙ";
        private const string delimeter = "";
        

        public override Type StorageType
        {
            get { return typeof (string); }
        }

        public override object ConvertToStorageType(object value)
        {
            if (value == null) return null;
            string s = ((Dictionary<string, string>) value).Aggregate<KeyValuePair<string, string>, string>(null, (current, o) => current + (o.Key + keyDelimeter + o.Value + delimeter));

            if (s != null) return s.TrimEnd(delimeter.ToCharArray());
            return null;
        }

        public override object ConvertFromStorageType(object value)
        {
            if (value == null) return null;
            string[] split = value.ToString().Split(delimeter.ToCharArray());
            if (value.ToString().IndexOf(keyDelimeter) > -1){
                return split.Select(s => s.Split(keyDelimeter.ToCharArray())).ToDictionary(strings => strings[0].TrimStart('['), strings => strings.Length == 1 ? null : strings[1].Trim().TrimEnd(']'));
            }
            return new Dictionary<string, string>();
        }
    }
}