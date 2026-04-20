// features/tooltip/tooltipInit.ts
import { bindValue } from "cs2/api";
import { applyTooltipBlocker, removeTooltipBlocker } from "./tooltipService";

const GROUP = "DisableHover";

export function initializeTooltip() {
    const binding = bindValue<boolean>(GROUP, "GetTooltipsEnabled");

    if (binding.value) {
        removeTooltipBlocker();
    } else {
        applyTooltipBlocker();
    }
}