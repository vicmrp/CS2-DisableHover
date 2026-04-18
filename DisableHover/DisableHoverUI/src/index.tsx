import { ModRegistrar } from "cs2/modding";
import { injectBelowWhatsNew } from "./helper/find-element";
import { enableDeepInspector } from "./helper/inspect-element";
import { testCommunication } from "helper/test-communication";
import { initializeTooltipBlocker } from "mods/tooltipBlocker";


const register: ModRegistrar = () => {
    setInterval(() => {
        injectBelowWhatsNew();
    }, 100);

    initializeTooltipBlocker();
};

export default register;