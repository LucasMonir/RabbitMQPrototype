namespace Utils
{
	public static class LogTypes
	{
		public enum TYPES
		{
			info,
			email
		}

		public static List<TYPES> GetEnums()
		{
			return Enum.GetValues(typeof(TYPES)).Cast<TYPES>().ToList();
		}
	}
}