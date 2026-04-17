import React, { useEffect, useState } from "react";
import {
    areTooltipsDisabled,
    initializeTooltipBlocker,
    setTooltipsDisabled
} from "./tooltipBlocker";
import { FloatingButton } from "cs2/ui";

export const TooltipToggle = () => {
    const [disabled, setDisabled] = useState<boolean>(areTooltipsDisabled());

    useEffect(() => {
        initializeTooltipBlocker();
    }, []);

    const onToggle = () => {
        const nextValue = !disabled;
        setTooltipsDisabled(nextValue);
        setDisabled(nextValue);
    };

    return (
        <div
            style={{
                position: "absolute",
                top: "96px",
                right: "16px",
                zIndex: 9999,
                display: "flex",
                flexDirection: "column",
                gap: "6px",
                padding: "8px",
                borderRadius: "8px",
                background: "rgba(0, 0, 0, 0.65)",
                color: "white",
                minWidth: "180px",
                fontFamily: "sans-serif"
            }}
        >
            <div style={{ fontSize: "12px", fontWeight: 700 }}>
                DisableHover
            </div>

             <FloatingButton
                onClick={onToggle}
            />
            

            <div style={{ fontSize: "11px", opacity: 0.85 }}>
                Status: {disabled ? "Tooltips hidden" : "Tooltips visible"}
            </div>
        </div>
    );
};