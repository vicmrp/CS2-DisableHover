import { setTooltipsDisabled, areTooltipsDisabled } from "../mods/tooltipBlocker";

export function findSettingRowByText(text: string): HTMLElement | null {
    const labels = document.querySelectorAll("div[class*='label_']");

    for (const label of labels) {
        if (label.textContent?.includes(text)) {
            return label.closest("div[class*='field']");
        }
    }

    return null;
}

function updateToggleVisual(el: HTMLElement, enabled: boolean) {
    el.classList.toggle("checked", enabled);
    el.classList.toggle("unchecked", !enabled);

    // update checkmark inside
    const checkmark = el.querySelector("[class*='checkmark']");
    if (checkmark) {
        checkmark.classList.toggle("checked", enabled);
    }
}

export function injectBelowWhatsNew() {
    const row = findSettingRowByText("What's New");

    if (!row) return;
    if (row.nextElementSibling?.id === "my-toggle-row") return;

    const container = row.parentElement;
    if (!container) return;

    // 🔽 REPLACE YOUR OLD CLONE LOGIC WITH THIS BLOCK
    const clone = row.cloneNode(true) as HTMLElement;
    clone.id = "my-toggle-row";

    // remove React bindings (important)
    const cleanClone = clone.cloneNode(true) as HTMLElement;

    const label = cleanClone.querySelector("div[class*='label']");
    if (label) label.textContent = "Disable Tooltips";

    const toggle = cleanClone.querySelector(
        "[role='button'], button, [class*='toggle']"
    );

    if (toggle) {
        (toggle as HTMLElement).style.pointerEvents = "auto";

        updateToggleVisual(toggle as HTMLElement, areTooltipsDisabled());

        toggle.addEventListener("click", (e) => {
            e.stopPropagation();
            e.preventDefault();

            const next = !areTooltipsDisabled();
            setTooltipsDisabled(next);

            updateToggleVisual(toggle as HTMLElement, next);

            console.log("[Toggle] Clicked:", next);
        });
    }

    container.insertBefore(cleanClone, row.nextSibling);
}

