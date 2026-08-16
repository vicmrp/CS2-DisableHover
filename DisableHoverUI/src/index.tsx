import { ModRegistrar } from "cs2/modding";
import { listenTooltipChanges } from "features/tooltip/tooltipBinding";
import { initializeTooltip } from "features/tooltip/tooltipInit";
import { DisableHoverButton } from "features/toolbar/DisableHoverButton";

const register: ModRegistrar = (moduleRegistry) => {
    listenTooltipChanges();
    initializeTooltip();

    // Native supported append point; does not replace or patch vanilla UI.
    moduleRegistry.append("GameTopRight", DisableHoverButton);
};

export default register;
