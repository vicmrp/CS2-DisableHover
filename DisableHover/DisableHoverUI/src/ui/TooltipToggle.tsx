// ui/TooltipToggle.tsx
import React from "react";
import { useTooltipBinding } from "../features/tooltip/tooltipBinding";
import { applyTooltipBlocker, removeTooltipBlocker } from "../features/tooltip/tooltipService";

export function TooltipToggle() {
    const [value, setValue] = useTooltipBinding();

    const toggle = () => {
        const next = !value;

        if (next) removeTooltipBlocker();
        else applyTooltipBlocker();

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