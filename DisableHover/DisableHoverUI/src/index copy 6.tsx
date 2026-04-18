import { ModRegistrar } from "cs2/modding";
import { injectBelowWhatsNew } from "./helper/find-element";

const register: ModRegistrar = () => {

    // Run continuously (UI is dynamic)
    setInterval(() => {
        injectBelowWhatsNew();
    }, 1000);

    console.log("[DisableHover] UI injector running");
};

export default register;