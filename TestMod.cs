using Terraria.ModLoader;

namespace TestMod
{
	public class TestMod : Mod
	{
		public TestMod()
        {
			Instance = this;
        }

		internal static TestMod Instance { get; private set; }
	}
}