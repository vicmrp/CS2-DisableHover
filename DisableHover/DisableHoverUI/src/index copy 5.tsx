import { ModRegistrar } from "cs2/modding";
import { injectBelowWhatsNew } from "./helper/find-element";

const register: ModRegistrar = (moduleRegistry) => {

    // Run continuously because CS2 re-renders UI
    setInterval(() => {
        injectBelowWhatsNew();
    }, 1000);

    console.log("[DisableHover] Injector initialized");
};

export default register;