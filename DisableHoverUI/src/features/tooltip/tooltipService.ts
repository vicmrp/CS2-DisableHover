// features/tooltip/tooltipService.ts
let tooltipStyle: HTMLStyleElement | null = null;

const TOOLTIP_CSS = `
[class*="balloon"],
[class*="tooltip-base"],
[class*="tooltip-fade"] {
    display: none !important;
    pointer-events: none !important;
}
`;

export function applyTooltipBlocker() {
    if (tooltipStyle) return;

    tooltipStyle = document.createElement("style");
    tooltipStyle.textContent = TOOLTIP_CSS;
    document.head.appendChild(tooltipStyle);
}

export function removeTooltipBlocker() {
    tooltipStyle?.remove();
    tooltipStyle = null;
}