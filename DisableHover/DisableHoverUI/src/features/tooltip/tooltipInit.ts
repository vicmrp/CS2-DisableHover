// features/tooltip/tooltipInit.ts
import { bindValue } from "cs2/api";
import { applyTooltipBlocker, removeTooltipBlocker } from "./tooltipService";

const GROUP = "DisableHover";

export function initializeTooltip() {
    const getDisableUIToolTips = bindValue<boolean>(GROUP, "GetDisableUIToolTips");

    console.log("[initializeTooltip] getDisableUIToolTips: ", getDisableUIToolTips.value);
    if (getDisableUIToolTips.value === true) {        
        removeTooltipBlocker();
    } else {        
        applyTooltipBlocker();
    }
}