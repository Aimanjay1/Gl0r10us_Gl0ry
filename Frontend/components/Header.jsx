"use client"
import { Avatar } from "@/components/ui/avatar";
import Link from "next/link";
import { SidebarTrigger } from "./ui/sidebar";
import { useUser } from "./UserProvider";

export default function Header(props) {
    const { user, refresh } = useUser()
    console.log("user,", user)

    return (
        <div className="flex w-full bg-accent py-4 items-center justify-between ">
            <div className="flex gap-4 px-2 ">
                {/* <Button className="bg-accent hover:bg-accent/20 hover:-translate-y-0.5 hover:shadow-xl">
                    <HamburgerMenuIcon className="text-primary w-8 h-8" />
                </Button> */}
                <SidebarTrigger className={"hover:-translate-y-[1px] hover:shadow-sm hover:cursor-pointer"} />
                <h1 className="flex w-full font-bold text-2xl text-identity"><Link href={"/"}>TunaiFlow</Link></h1>
            </div>

            <div className=" pr-4 flex items-center gap-2" >
                <Avatar className={"bg-primary h-6 w-6 lg:h-8 lg:w-8"} src={null} />
                <div className="text-sm flex flex-col justify-center">
                    <p className="font-bold ">{user}</p>
                    <p className="text-ellipsis">{user ? (user.email || "Shit") : "Shit"}</p>
                </div>
            </div>
        </div>
    )
}