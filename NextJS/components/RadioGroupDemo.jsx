import { Label } from "@/components/ui/label"
import {
    RadioGroup,
    RadioGroupItem,
} from "@/components/ui/radio-group"

export function RadioGroupDemo() {
    return (
        <RadioGroup defaultValue="comfortable" className={"flex p-4"} >
            <div className="flex items-center gap-3">
                <RadioGroupItem value="default" id="r1" />
                <Label htmlFor="r1">Monthly</Label>
            </div>
            <div className="flex items-center gap-3">
                <RadioGroupItem value="comfortable" id="r2" />
                <Label htmlFor="r2">Yearly</Label>
            </div>
        </RadioGroup>
    )
}
