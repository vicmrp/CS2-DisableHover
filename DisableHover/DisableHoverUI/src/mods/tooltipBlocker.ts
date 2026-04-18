import { trigger, bindValue } from "cs2/api";
const STORAGE_KEY = "disablehover-tooltips-disabled";
const GROUP = "DisableHover";
let tooltipStyle: HTMLStyleElement | null = null;

const TOOLTIP_CSS = `
    [class*="balloon"],
    [class*="tooltip-base"],
    [class*="tooltip-fade"] {
        display: none !important;
        pointer-events: none !important;
    }
`;

export function areTooltipsDisabled(): boolean {
    return localStorage.getItem(STORAGE_KEY) === "true";
}

export function setTooltipsDisabled(value: boolean): void {
    localStorage.setItem(STORAGE_KEY, String(value));

    if (value) {

        const binding = bindValue<boolean>(GROUP, "GetTooltipsDisabled");
        binding.subscribe((value) => {
            console.log("[UI] Received value from C#:", value);
        });
        trigger(GROUP, "SetTooltipsDisabled", value);

        applyTooltipBlocker();
    } else {
        const binding = bindValue<boolean>(GROUP, "GetTooltipsDisabled");
        binding.subscribe((value) => {
            console.log("[UI] Received value from C#:", value);
        });
        trigger(GROUP, "SetTooltipsDisabled", false);
        removeTooltipBlocker();
    }
}

export function applyTooltipBlocker(): void {
    if (tooltipStyle) return;

    tooltipStyle = document.createElement("style");
    tooltipStyle.setAttribute("data-disablehover-tooltips", "true");
    tooltipStyle.textContent = TOOLTIP_CSS;
    document.head.appendChild(tooltipStyle);

    console.log("[DisableHover] Tooltip blocker enabled");
}

export function removeTooltipBlocker(): void {
    if (!tooltipStyle) return;

    tooltipStyle.remove();
    tooltipStyle = null;

    console.log("[DisableHover] Tooltip blocker disabled");
}

export function initializeTooltipBlocker(): void {
    console.log("Initializing Tooltips")
    if (areTooltipsDisabled()) {
        applyTooltipBlocker();
    } else {
        removeTooltipBlocker();
    }
}