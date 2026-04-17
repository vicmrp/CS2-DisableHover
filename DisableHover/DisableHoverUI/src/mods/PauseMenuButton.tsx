import React, { useEffect, useState } from "react";
import { MenuButton } from "cs2/ui";
import {
    areTooltipsDisabled,
    setTooltipsDisabled,
    initializeTooltipBlocker
} from "./tooltipBlocker";

export const PauseMenuButton = () => {
    const [disabled, setDisabled] = useState(areTooltipsDisabled());

    useEffect(() => {
        initializeTooltipBlocker();
    }, []);

    const onClick = () => {
        const next = !disabled;
        setTooltipsDisabled(next);
        setDisabled(next);
    };

    return (
        <MenuButton onClick={onClick}>
            {disabled ? "Enable Tooltips" : "Disable Tooltips"}
        </MenuButton>
    );
};