import { ModRegistrar } from "cs2/modding";
import { enableDeepInspector, inspectElement, inspectWithParents } from "dev/inspect-element";
import { injectTooltipToggle } from "injection/injectToggle";


const register: ModRegistrar = (moduleRegistry) => {
    enableDeepInspector()
    

    moduleRegistry.extend(
        "game-ui/menu/components/options-screen/option-page/option-page.tsx",
        "OptionPage",
        (Component) => {
            return (props) => {
                injectTooltipToggle(); // handles observer + DOM logic
                return <Component {...props} />;
            };
        }
    );
};

export default register;