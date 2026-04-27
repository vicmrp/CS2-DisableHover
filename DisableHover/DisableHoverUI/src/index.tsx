import { ModRegistrar } from "cs2/modding";
import { enableDeepInspector, inspectElement, inspectWithParents } from "dev/inspect-element";
import { useTooltipBinding } from "features/tooltip/tooltipBinding";
import { initializeTooltip } from "features/tooltip/tooltipInit"

const register: ModRegistrar = (moduleRegistry) => {
    
    // enableDeepInspector()
    // useTooltipBinding()

    // check if the tooltip is enabled on start
    initializeTooltip()

};

export default register;