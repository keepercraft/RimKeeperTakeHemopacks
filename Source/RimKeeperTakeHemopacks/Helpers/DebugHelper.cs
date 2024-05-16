using Verse;

namespace Keepercraft.RimKeeperTakeHemopacks.Helpers
{
    public static class DebugHelper
    {
        private static string _header = "Debug"; 
        public static bool Active = KeeperModSettings.DebugLog;

        public static void SetHeader(string text) => _header = string.Format("[{0}] ", text);

        public static void Message(string text, params object[] args)
        {
            if (Active)
            {
                Log.Message(_header + string.Format(text, args));
            }
        }
    }
}
