// features/tooltip/tooltipInit.ts
import { bindValue } from "cs2/api";
import { applyTooltipBlocker, removeTooltipBlocker } from "./tooltipService";

const GROUP = "DisableHover";

export function initializeTooltip() {
    const getTooltipsEnabled = bindValue<boolean>(GROUP, "GetTooltipsEnabled");

    console.log("[initializeTooltip] getTooltipsEnabled: ", getTooltipsEnabled.value);
    if (getTooltipsEnabled.value) {        
        removeTooltipBlocker();
    } else {
        applyTooltipBlocker();
    }
}