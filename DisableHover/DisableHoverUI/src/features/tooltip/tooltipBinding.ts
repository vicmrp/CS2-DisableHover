import { bindValue } from "cs2/api";
import { applyTooltipBlocker, removeTooltipBlocker } from "features/tooltip/tooltipService";

const GROUP = "DisableHover";

export function listenTooltipChanges() {
    const binding = bindValue<boolean>(GROUP, "GetTooltipsEnabled");

    binding.subscribe((value) => {
        console.log("[UI] C# updated value →", value);
    });

    if (binding.value) {
        applyTooltipBlocker()
    } else {
        removeTooltipBlocker()
    }

    console.log("[UI] Initial value →", binding.value);
}