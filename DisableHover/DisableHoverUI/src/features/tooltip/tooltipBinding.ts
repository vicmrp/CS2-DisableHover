import { bindValue } from "cs2/api";
import { applyTooltipBlocker, removeTooltipBlocker } from "features/tooltip/tooltipService";

const GROUP = "DisableHover";

export function listenTooltipChanges() {
    const getTooltipsEnabled = bindValue<boolean>(GROUP, "GetTooltipsEnabled");

    getTooltipsEnabled.subscribe((value) => {
        console.log("[UI] C# updated value →", value);
    });

    if (getTooltipsEnabled.value) {
        console.log("[UI] C# applying tooltip blocker");        
        applyTooltipBlocker()
    } else {
        console.log("[UI] C# removing tooltip blocker");
        removeTooltipBlocker()
    }
}