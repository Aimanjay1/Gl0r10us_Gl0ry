import { HamburgerMenuIcon } from "@radix-ui/react-icons"
import { Button } from "@/components/ui/button";
import { Avatar } from "@/components/ui/avatar";

export default function Footer(props) {
    return (
        <div className="flex w-full bg-accent py-4 text-2xl items-center justify-between ">

            <div className="flex gap-4 px-4 ">
                <Button className="bg-accent hover:bg-accent/20 hover:-translate-y-2 hover:shadow-xl">
                    <HamburgerMenuIcon className="text-primary w-8 h-8" />
                </Button>
                <h1 className="flex w-full font-bold text-identity">GloriousGlory</h1>
            </div>

            <div className="px-4 flex items-center gap-2">
                <Avatar className={"bg-primary"} src={null} />
                <div className="text-sm flex flex-col justify-center">
                    <p className="font-bold">username</p>
                    <p>username@email.com</p>
                </div>
            </div>

        </div>
    )
}