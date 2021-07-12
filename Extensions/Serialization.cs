using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClickQuest.Extensions
{
	public static class Serialization
	{
		public static string SerializeData<T1,T2>(IDictionary<T1,T2> source)
		{
			var builder = new StringBuilder();

			int i = 0;

			foreach (var pair in source)
			{
				builder.Append($"{pair.Value.ToString()}");

				if (i != source.Count() - 1)
				{
					builder.Append(',');
				}

				i++;
			}

			return builder.ToString();
		}

		public static void DeserializeData<T1,T2>(string source, IDictionary<T1,T2> destination) where T1:System.Enum where T2: struct
		{
			if (string.IsNullOrEmpty(source))
			{
				return;
			}

			int indexOfComma = -1;
			int i = 0;

			while ((indexOfComma = source.IndexOf(',')) != -1)
			{
				string value = source.Substring(0, indexOfComma);
				source = source.Remove(0, indexOfComma+1);
				destination[(T1)(object)i] = (T2)Convert.ChangeType(value, typeof(T2));
				i++;
			}

			destination[(T1)(object)i]=(T2)Convert.ChangeType(source, typeof(T2));			
		}
	}
}