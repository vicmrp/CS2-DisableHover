import { setTooltipsDisabled, areTooltipsDisabled } from "../mods/tooltipBlocker";

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
 * Update CS2 toggle visual state (checked / unchecked)
 */
function updateToggleVisual(el: HTMLElement, enabled: boolean) {
    el.classList.toggle("checked", enabled);
    el.classList.toggle("unchecked", !enabled);

    // update inner checkmark (visual)
    const checkmark = el.querySelector("[class*='checkmark']");
    if (checkmark) {
        checkmark.classList.toggle("checked", enabled);
    }
}

/**
 * Inject our custom toggle into Interface settings
 */
export function injectBelowWhatsNew() {
    const row = findSettingRowByText("What's New");

    if (!row) return;

    // prevent duplicates
    if (row.nextElementSibling?.id === "my-toggle-row") return;

    const container = row.parentElement;
    if (!container) return;

    // clone an existing CS2 row (keeps styling)
    const clone = row.cloneNode(true) as HTMLElement;
    clone.id = "my-toggle-row";

    // remove React bindings (VERY IMPORTANT)
    const cleanClone = clone.cloneNode(true) as HTMLElement;

    // ✅ Set correct label
    const label = cleanClone.querySelector("div[class*='label']");
    if (label) label.textContent = "Enable Tooltips";

    // find actual toggle button
    const toggle = cleanClone.querySelector(
        "[role='button'], button, [class*='toggle']"
    ) as HTMLElement | null;

    if (toggle) {
        toggle.style.pointerEvents = "auto";

        // ✅ Enabled = inverse of disabled
        const enabled = !areTooltipsDisabled();

        updateToggleVisual(toggle, enabled);

        toggle.addEventListener("click", (e) => {
            e.stopPropagation();
            e.preventDefault();

            const currentEnabled = !areTooltipsDisabled();
            const nextEnabled = !currentEnabled;

            // invert for storage
            setTooltipsDisabled(!nextEnabled);

            updateToggleVisual(toggle, nextEnabled);

            console.log("[Toggle] Enable Tooltips:", nextEnabled);
        });
    }

    container.insertBefore(cleanClone, row.nextSibling);
}