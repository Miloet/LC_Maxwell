using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LC_Maxwell.Patches;

namespace LC_Maxwell
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class MaxwellReplacerMod : BaseUnityPlugin
    {
        private const string modGUID = "Mellowdy.MaxwellReplacer";
        private const string modName = "MaxwellReplacer";
        private const string modVersion = "1.0.0";


        private readonly Harmony harmony = new Harmony(modGUID);

        public static ManualLogSource mls;

        private static MaxwellReplacerMod instance;

        void Awake()
        {
            if (instance == null) instance = this;

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            mls.LogInfo("Maxwell is in your walls");


            harmony.PatchAll(typeof(MaxwellReplacerMod));
            harmony.PatchAll(typeof(ItemReplacerPatch));
        }
    }
}
