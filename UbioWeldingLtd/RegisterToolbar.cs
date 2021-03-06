using ToolbarControl_NS;
using UnityEngine;

namespace UbioWeldingLtd
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {

        void Start()
        {
            Debug.Log("UbioWeldingLtd RegisterToolbar");
            ToolbarControl.RegisterMod(EditorToolbar.MODID, Constants.weldManufacturer);          
        }
    }
}
