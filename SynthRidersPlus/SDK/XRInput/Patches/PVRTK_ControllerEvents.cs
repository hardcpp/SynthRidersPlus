using HarmonyLib;

namespace SynthRidersPlus.SDK.XRInput.Patches
{
    [HarmonyPatch]
    public class PVRTK_ControllerEvents : VRTK.VRTK_ControllerEvents
    {
        [HarmonyPatch(typeof(VRTK.VRTK_ControllerEvents))]
        [HarmonyPatch(nameof(VRTK.VRTK_ControllerEvents.OnTouchpadAxisChanged))]
        [HarmonyPostfix]
        internal static void OnTouchpadAxisChanged_Postfix(VRTK.VRTK_ControllerEvents __instance, ref VRTK.ControllerInteractionEventArgs e)
            => SRController.ControllerEvents_TouchpadAxisChanged(__instance, e);

        [HarmonyPatch(typeof(VRTK.VRTK_ControllerEvents))]
        [HarmonyPatch(nameof(VRTK.VRTK_ControllerEvents.OnTriggerClicked))]
        [HarmonyPostfix]
        internal static void OnTriggerClicked_Postfix(VRTK.VRTK_ControllerEvents __instance, ref VRTK.ControllerInteractionEventArgs e)
            => SRController.ControllerEvents_TriggerClicked(__instance, e);

        [HarmonyPatch(typeof(VRTK.VRTK_ControllerEvents))]
        [HarmonyPatch(nameof(VRTK.VRTK_ControllerEvents.OnTriggerUnclicked))]
        [HarmonyPostfix]
        internal static void OnTriggerUnclicked_Postfix(VRTK.VRTK_ControllerEvents __instance, ref VRTK.ControllerInteractionEventArgs e)
            => SRController.ControllerEvents_TriggerUnclicked(__instance, e);

        [HarmonyPatch(typeof(VRTK.VRTK_ControllerEvents))]
        [HarmonyPatch(nameof(VRTK.VRTK_ControllerEvents.OnTriggerAxisChanged))]
        [HarmonyPostfix]
        internal static void OnTriggerAxisChanged_Postfix(VRTK.VRTK_ControllerEvents __instance, ref VRTK.ControllerInteractionEventArgs e)
            => SRController.ControllerEvents_TriggerAxisChanged(__instance, e);
    }
}
