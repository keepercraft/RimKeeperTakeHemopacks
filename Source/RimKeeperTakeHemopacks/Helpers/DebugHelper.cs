using Keepercraft.RimKeeperTakeHemopacks;
using Verse;

namespace RimKeeperTakeHemopacks.Helpers
{
    public static class DebugHelper
    {
        public static bool Active = KeeperModSettings.DebugLog;

        public static void Message(string text, params object[] args)
        {
            if (Active)
            {
                Log.Message(string.Format(text, args));
            }
        }
    }
}
