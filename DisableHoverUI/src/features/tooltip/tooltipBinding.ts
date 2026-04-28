import { bindValue } from "cs2/api";
import { applyTooltipBlocker, removeTooltipBlocker } from "features/tooltip/tooltipService";

const GROUP = "DisableHover";

export function listenTooltipChanges() {
    const getDisableUIToolTips = bindValue<boolean>(GROUP, "GetDisableUIToolTips");

    getDisableUIToolTips.subscribe((value) => {
        console.log("[UI] C# updaated value →", value);

        if (value) {
            console.log("[UI] C# applying tooltip blocker");
            applyTooltipBlocker()
        } else {
            console.log("[UI] C# removing tooltip blocker");
            removeTooltipBlocker()
        }
    });
}