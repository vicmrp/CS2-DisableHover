// injection/injectToggle.ts
import { createRoot } from "react-dom/client";
import { TooltipToggle } from "../ui/TooltipToggle";

export function injectTooltipToggle() {
    let injected = false;

    const inject = () => {
        if (injected) return;

        const labels = document.querySelectorAll("div");

        for (const el of labels) {
            if (el.textContent?.includes("What's New")) {

                const row = el.closest("div[class*='field']");
                if (!row) continue;

                if (row.nextElementSibling?.id === "tooltip-toggle-row") return;

                const clone = row.cloneNode(true) as HTMLElement;
                clone.id = "tooltip-toggle-row";

                const label = clone.querySelector("div[class*='label']");
                if (label) label.textContent = "Show Tooltips on Hover";

                clone.lastElementChild?.remove();

                const mount = document.createElement("div");
                clone.appendChild(mount);

                createRoot(mount).render(<TooltipToggle />);

                row.parentElement?.insertBefore(clone, row.nextElementSibling);

                injected = true;
                return;
            }
        }
    };

    inject();

    const observer = new MutationObserver(inject);
    observer.observe(document.body, { childList: true, subtree: true });
}