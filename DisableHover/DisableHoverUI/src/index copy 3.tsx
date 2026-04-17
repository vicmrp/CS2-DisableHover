import { ModRegistrar } from "cs2/modding";
import { TooltipToggle } from "mods/TooltipToggle";




// const register: ModRegistrar = (moduleRegistry) => {
//     moduleRegistry.append("GameTopRight", TooltipToggle);
// };


const register: ModRegistrar = (moduleRegistry) => {
    console.log(moduleRegistry.find(/pause/i));
};

export default register;