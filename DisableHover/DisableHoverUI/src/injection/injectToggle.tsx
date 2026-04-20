// injection/injectToggle.ts
import { createRoot } from "react-dom/client";
import { TooltipToggle } from "../ui/TooltipToggle";

/**
 * Known translations of "What's New"
 */
const WHATS_NEW_KEYS = [
    "what's new", "whats new", // English
    "neuigkeiten", // Deutsch
    "novedades", // Espanol (spanish)
    "nouveautés", "nouveautes", // French
    "nowości", "nowosci", // Polski
    "novidades", // Portugues
    "что нового", // Russian
    "新内容", "最新消息", // Chinese
];

/**
 * Normalize + match text
 */
function isWhatsNewLabel(text: string): boolean {
    const t = text.toLowerCase().trim();
    return WHATS_NEW_KEYS.some(k => t.includes(k));
}

export function injectTooltipToggle() {
    let injected = false;

    const inject = () => {
        if (injected) return;

        const elements = document.querySelectorAll("div");

        for (const el of elements) {
            const text = el.textContent;
            if (!text) continue;

            // 🔍 Language-safe detection
            if (!isWhatsNewLabel(text)) continue;

            const row = el.closest("div[class*='field']") as HTMLElement | null;
            if (!row) continue;

            // Prevent duplicate injection
            if (row.nextElementSibling?.id === "tooltip-toggle-row") return;

            // Clone existing row
            const clone = row.cloneNode(true) as HTMLElement;
            clone.id = "tooltip-toggle-row";

            // Replace label text
            const label = clone.querySelector("div[class*='label']");
            if (label) label.textContent = "Show Tooltips on Hover";

            // Remove original control (toggle/checkbox/etc.)
            clone.lastElementChild?.remove();

            // Mount React toggle
            const mount = document.createElement("div");
            clone.appendChild(mount);

            createRoot(mount).render(<TooltipToggle />);

            // Insert into DOM
            row.parentElement?.insertBefore(clone, row.nextElementSibling);

            injected = true;
            return;
        }
    };

    // Initial run
    inject();

    // Observe UI changes (CS2 UI is dynamic)
    const observer = new MutationObserver(inject);
    observer.observe(document.body, {
        childList: true,
        subtree: true
    });
}