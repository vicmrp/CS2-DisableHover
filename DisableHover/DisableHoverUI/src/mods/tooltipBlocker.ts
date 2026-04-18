import { call, trigger } from "cs2/api";

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

export async function areTooltipsDisabled(): Promise<boolean> {
    try {
        return await call<boolean>(GROUP, "GetTooltipsDisabled");
    } catch (e) {
        console.error("[DisableHover] Get failed", e);
        return false;
    }
}

export async function setTooltipsDisabled(value: boolean): Promise<void> {
    try {
        await call<void>(GROUP, "SetTooltipsDisabled", value);
    } catch (e) {
        console.error("[DisableHover] Set failed", e);
    }
}

export function applyTooltipBlocker(): void {
    if (tooltipStyle) return;

    tooltipStyle = document.createElement("style");
    tooltipStyle.textContent = TOOLTIP_CSS;
    document.head.appendChild(tooltipStyle);

    console.log("[DisableHover] ENABLED");
}

export function removeTooltipBlocker(): void {
    if (!tooltipStyle) return;

    tooltipStyle.remove();
    tooltipStyle = null;

    console.log("[DisableHover] DISABLED");
}

export async function initializeTooltipBlocker(): Promise<void> {
    const disabled = await areTooltipsDisabled();

    if (disabled) applyTooltipBlocker();
    else removeTooltipBlocker();
}