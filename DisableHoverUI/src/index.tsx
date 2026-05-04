import { ModRegistrar } from "cs2/modding";
import { enableDeepInspector, enableDeepInspectOnHover } from "dev/inspect-element";
import { listenTooltipChanges } from "features/tooltip/tooltipBinding";
import { initializeTooltip } from "features/tooltip/tooltipInit";



const register: ModRegistrar = (moduleRegistry) => {
    
    console.log("Registering UI Values");
    // enableDeepInspector()
    enableDeepInspectOnHover();   // hover-based
    listenTooltipChanges();

    // check if the tooltip is enabled on start
    initializeTooltip()

};

export default register;