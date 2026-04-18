import {
    setTooltipsDisabled,
    areTooltipsDisabled,
    applyTooltipBlocker,
    removeTooltipBlocker
} from "../mods/tooltipBlocker";

/**
 * Find a settings row by visible label text
 */
export function findSettingRowByText(text: string): HTMLElement | null {
    const labels = document.querySelectorAll("div[class*='label_']");

    for (const label of labels) {
        if (label.textContent?.includes(text)) {
            return label.closest("div[class*='field']");
        }
    }

    return null;
}

/**
 * Update CS2 toggle visual state
 */
function updateToggleVisual(el: HTMLElement, enabled: boolean) {
    el.classList.toggle("checked", enabled);
    el.classList.toggle("unchecked", !enabled);

    const checkmark = el.querySelector("[class*='checkmark']");
    if (checkmark) {
        checkmark.classList.toggle("checked", enabled);
    }
}

/**
 * Inject custom toggle into settings
 */
export function injectBelowWhatsNew() {
    const row = findSettingRowByText("What's New");
    if (!row) return;

    // prevent duplicates
    if (row.nextElementSibling?.id === "my-toggle-row") return;

    const container = row.parentElement;
    if (!container) return;

    // clone row
    const clone = row.cloneNode(true) as HTMLElement;
    clone.id = "my-toggle-row";

    const cleanClone = clone.cloneNode(true) as HTMLElement;

    // change label
    const label = cleanClone.querySelector("div[class*='label']");
    if (label) label.textContent = "Enable Tooltips";

    const toggle = cleanClone.querySelector(
        "[role='button'], button, [class*='toggle']"
    ) as HTMLElement | null;

    if (toggle) {
        toggle.style.pointerEvents = "auto";

        // init state from C#
        areTooltipsDisabled().then(disabled => {
            const enabled = !disabled;
            updateToggleVisual(toggle, enabled);
        });

        toggle.addEventListener("click", async (e) => {
            e.stopPropagation();
            e.preventDefault();

            const currentDisabled = await areTooltipsDisabled();
            const nextDisabled = !currentDisabled;

            // save via C#
            setTooltipsDisabled(nextDisabled);

            // update UI
            updateToggleVisual(toggle, !nextDisabled);

            // apply/remove CSS immediately
            if (nextDisabled) {
                applyTooltipBlocker();
            } else {
                removeTooltipBlocker();
            }

            console.log("[DisableHover] Tooltips disabled:", nextDisabled);
        });
    }

    container.insertBefore(cleanClone, row.nextSibling);
}