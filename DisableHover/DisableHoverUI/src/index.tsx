import { ModRegistrar } from "cs2/modding";
import { injectBelowWhatsNew } from "./helper/find-element";
import { enableDeepInspector } from "./helper/inspect-element";
import { initializeTooltipBlocker } from "./mods/tooltipBlocker";

const register: ModRegistrar = () => {

    setInterval(() => {
        injectBelowWhatsNew();
    }, 1);

    enableDeepInspector();

    // sync CSS with C# state on load
    initializeTooltipBlocker();
};

export default register;