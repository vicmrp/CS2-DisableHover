import React, { useEffect, useState } from "react";
import { MenuButton } from "cs2/ui";
import {
    areTooltipsDisabled,
    setTooltipsDisabled,
    initializeTooltipBlocker
} from "./tooltipBlocker";

export const PauseMenuButton = () => {
    const [enabled, setEnabled] = useState(!areTooltipsDisabled());

    useEffect(() => {
        initializeTooltipBlocker();
    }, []);

    const onClick = () => {
        const nextEnabled = !enabled;

        setTooltipsDisabled(!nextEnabled); // invert
        setEnabled(nextEnabled);
    };

    return (
        <MenuButton onClick={onClick}>
            {enabled ? "Disable Tooltips" : "Enable Tooltips"}
        </MenuButton>
    );
};