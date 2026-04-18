import { ModRegistrar } from "cs2/modding";
import React from "react";
import { PauseMenuButton } from "./mods/PauseMenuButton";
import { logElementTree } from "./helper/find-element";



const ExtendPauseMenu = (Component: any) => {
    return (props: any) => {
        const children = React.Children.toArray(props.children);

        return React.createElement(
            Component,
            props,
            ...children,
            React.createElement(PauseMenuButton)
        );
    };
};





const register: ModRegistrar = (moduleRegistry) => {
    moduleRegistry.extend(
        "game-ui/menu/components/pause-menu/pause-menu.tsx",
        "PauseMenu",
        ExtendPauseMenu
    );
};

export default register;