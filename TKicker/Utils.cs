using System;

namespace ServerHasMoved
{
	public static class Utils
	{
		public static T GetParam<T>(this string[] args, string find, object Default = default(object))
		{
			return args.GetParam<T>(new string[] { find }, Default);
		}

		public static T GetParam<T>(this string[] args, string[] find, object Default = default(object))
		{
			for (int i = 0; i < args.Length; i++)
			{
				for (int j = 0; j < find.Length; j++)
				{
					if (args[i] == find[j])
					{
						if (args.Length < i + 1)
							break;
						return (T)Convert.ChangeType(args[i + 1], typeof(T));
					}
				}
			}
			return (T)Convert.ChangeType(Default, typeof(T));
		}
	}
}
