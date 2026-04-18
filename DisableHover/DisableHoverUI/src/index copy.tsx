import React from "react";
import { createRoot } from "react-dom/client";
import { ModRegistrar } from "cs2/modding";
import { bindValue, trigger } from "cs2/api";

const GROUP = "DisableHover";

/**
 * Native-like toggle component (uses your C# binding)
 */
function NativeLikeToggle() {
    const binding = bindValue<boolean>(GROUP, "GetTooltipsDisabled");
    const [value, setValue] = React.useState(false);

    React.useEffect(() => {
        const sub = binding.subscribe(v => setValue(v));
        return () => sub.dispose();
    }, []);

    const toggle = () => {
        const next = !value;
        trigger(GROUP, "SetTooltipsDisabled", next);
        setValue(next);
    };

    return (
        <div
            onClick={toggle}
            style={{
                width: "18px",
                height: "18px",
                border: "2px solid #6fa8dc",
                cursor: "pointer",
                display: "flex",
                alignItems: "center",
                justifyContent: "center"
            }}
        >
            {value && (
                <div
                    style={{
                        width: "10px",
                        height: "10px",
                        background: "#6fa8dc"
                    }}
                />
            )}
        </div>
    );
}

/**
 * Register mod
 */
const register: ModRegistrar = (moduleRegistry) => {

    moduleRegistry.extend(
        "game-ui/menu/components/options-screen/option-page/option-page.tsx",
        "OptionPage",
        (Component) => {

            console.log("✅ OptionPage EXTENSION REGISTERED");

            return (props) => {

                React.useEffect(() => {
                    const inject = () => {

                        const labels = document.querySelectorAll("div");

                        for (const el of labels) {
                            if (el.textContent?.includes("What's New")) {

                                const row = el.closest("div[class*='field']");
                                if (!row) continue;

                                // prevent duplicate injection
                                if (row.nextElementSibling?.id === "tooltip-toggle-row") return;

                                // 🔥 clone existing row (perfect styling)
                                const clone = row.cloneNode(true) as HTMLElement;
                                clone.id = "tooltip-toggle-row";

                                // replace label text
                                const label = clone.querySelector("div[class*='label']");
                                if (label) label.textContent = "Enable Tooltips";

                                // find right side (toggle container)
                                const right = clone.lastElementChild as HTMLElement;
                                if (right) right.remove(); // remove original toggle completely

                                const newRight = document.createElement("div");
                                newRight.style.display = "flex";
                                newRight.style.alignItems = "center";

                                const mount = document.createElement("div");
                                newRight.appendChild(mount);

                                clone.appendChild(newRight);

                                const root = createRoot(mount);
                                root.render(<NativeLikeToggle />);
                                row.parentElement?.insertBefore(
                                    clone,
                                    row.nextElementSibling
                                );

                                console.log("✅ Injected Enable Tooltips toggle");

                                return;
                            }
                        }
                    };

                    const interval = setInterval(inject, 400);
                    return () => clearInterval(interval);

                }, []);

                return <Component {...props} />;
            };
        }
    );
};

export default register;