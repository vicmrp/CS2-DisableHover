export function findSettingRowByText(text: string): HTMLElement | null {
    const labels = document.querySelectorAll("div[class*='label']");

    for (const label of labels) {
        if (label.textContent?.trim() === text) {
            return label.closest("div[class*='field']");
        }
    }

    return null;
}



export function injectBelowWhatsNew() {
    const row = findSettingRowByText('Show "What\'s New" Panel');

    if (!row) {
        console.log("[Inject] Target row not found");
        return;
    }

    if (row.nextElementSibling?.id === "my-toggle-row") return;

    const container = row.parentElement;
    if (!container) return;

    const wrapper = document.createElement("div");
    wrapper.id = "my-toggle-row";
    wrapper.className = row.className; // clone styling

    const label = document.createElement("div");
    label.className = "label_DGc label_ZLb";
    label.textContent = "Disable Tooltips";

    const toggle = document.createElement("input");
    toggle.type = "checkbox";
    toggle.checked = localStorage.getItem("disablehover-tooltips-disabled") === "true";

    toggle.onchange = () => {
        const value = toggle.checked;
        localStorage.setItem("disablehover-tooltips-disabled", String(value));

        console.log("[Toggle]", value);
    };

    wrapper.appendChild(label);
    wrapper.appendChild(toggle);

    container.insertBefore(wrapper, row.nextSibling);

    console.log("[Inject] Added toggle row");
}