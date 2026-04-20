// features/tooltip/tooltipBinding.ts
import { bindValue, trigger } from "cs2/api";
import React from "react";

const GROUP = "DisableHover";

export function useTooltipBinding(): [boolean, (v: boolean) => void] {
    const binding = bindValue<boolean>(GROUP, "GetTooltipsEnabled");
    const [value, setValue] = React.useState(binding.value);

    React.useEffect(() => {
        const sub = binding.subscribe(setValue);
        return () => sub.dispose();
    }, []);

    const update = (v: boolean) => {
        trigger(GROUP, "SetTooltipsEnabled", v);
        setValue(v);
    };

    return [value, update];
}