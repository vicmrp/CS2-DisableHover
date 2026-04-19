import React from "react";
import { createRoot } from "react-dom/client";
import { ModRegistrar } from "cs2/modding";
import { bindValue, trigger } from "cs2/api";
import { enableDeepInspector } from "helper/inspect-element";

const GROUP = "DisableHover";

/**
 * ✅ TRUE native-looking toggle (uses CS2 classes)
 */
function NativeLikeToggle() {
    const binding = bindValue<boolean>(GROUP, "GetTooltipsEnabled");
    const [value, setValue] = React.useState(false);

    React.useEffect(() => {
        const sub = binding.subscribe(v => setValue(v));
        return () => sub.dispose();
    }, []);

    const toggle = () => {
        const next = !value;
        trigger(GROUP, "SetTooltipsEnabled", next);
        setValue(next);
    };

    return (
        <div
            onClick={toggle}
            className={`toggle_cca item-mouse-states_Fmi toggle_th_ ${value ? "checked" : "unchecked"}`}
        >
            <div className={`checkmark_NXV ${value ? "checked" : ""}`} />
        </div>
    );
}

/**
 * Register mod
 */
const register: ModRegistrar = (moduleRegistry) => {

    enableDeepInspector();

    moduleRegistry.extend(
        "game-ui/menu/components/options-screen/option-page/option-page.tsx",
        "OptionPage",
        (Component) => {

            console.log("✅ OptionPage EXTENSION REGISTERED");

            return (props) => {

                React.useEffect(() => {
                    let injected = false;

                    const inject = () => {
                        if (injected) return;

                        const labels = document.querySelectorAll("div");

                        for (const el of labels) {
                            if (el.textContent?.includes("What's New")) {

                                const row = el.closest("div[class*='field']");
                                if (!row) continue;

                                if (row.nextElementSibling?.id === "tooltip-toggle-row") return;

                                // 🔥 clone full row (keeps layout + spacing)
                                const clone = row.cloneNode(true) as HTMLElement;
                                clone.id = "tooltip-toggle-row";

                                // ✅ change label text
                                const label = clone.querySelector("div[class*='label']");
                                if (label) label.textContent = "Show Tooltips on Hover";

                                // ❗ remove original toggle completely
                                const originalRight = clone.lastElementChild as HTMLElement;
                                if (originalRight) originalRight.remove();

                                // ✅ rebuild right side (clean)
                                const newRight = document.createElement("div");
                                newRight.style.display = "flex";
                                newRight.style.alignItems = "center";

                                const mount = document.createElement("div");
                                newRight.appendChild(mount);

                                clone.appendChild(newRight);

                                // ✅ mount native-like toggle
                                const root = createRoot(mount);
                                root.render(<NativeLikeToggle />);

                                // insert directly under "What's New"
                                row.parentElement?.insertBefore(
                                    clone,
                                    row.nextElementSibling
                                );

                                console.log("✅ Injected native toggle");
                                injected = true;
                                return;
                            }
                        }
                    };

                    // run immediately
                    inject();

                    // fallback observer (no delay now)
                    const observer = new MutationObserver(() => inject());
                    observer.observe(document.body, {
                        childList: true,
                        subtree: true
                    });

                    return () => observer.disconnect();

                }, []);

                return <Component {...props} />;
            };
        }
    );
};

export default register;