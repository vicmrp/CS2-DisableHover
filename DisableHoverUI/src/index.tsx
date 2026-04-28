import { ModRegistrar } from "cs2/modding";
import { listenTooltipChanges } from "features/tooltip/tooltipBinding";
import { initializeTooltip } from "features/tooltip/tooltipInit";



const register: ModRegistrar = (moduleRegistry) => {
    
    console.log("Registering UI Values");
    // enableDeepInspector()
    listenTooltipChanges();

    // check if the tooltip is enabled on start
    initializeTooltip()

};

export default register;