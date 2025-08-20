import {
    Sidebar,
    SidebarContent,
    SidebarFooter,
    SidebarGroup,
    SidebarHeader,
    SidebarMenu,
    SidebarMenuButton,
    SidebarMenuItem,
    SidebarSeparator,
    useSidebar
} from "@/components/ui/sidebar"
import Link from "next/link"
import LogoutButton from "./LogoutButton"
import { useIsMobile } from "@/hooks/use-mobile"
import { Separator } from "./ui/separator";

export function AppSidebar() {
    const sidebar = useSidebar();
    const isMobile = useIsMobile();

    function handleSidebarToggle() {
        if (isMobile) sidebar.toggleSidebar();
    }

    return (

        <Sidebar >
            <SidebarHeader >
                <SidebarGroup >
                    <h1 className="flex w-full font-bold text-2xl text-identity"><Link href={"/"}>TunaiFlow</Link></h1>
                </SidebarGroup>
            </SidebarHeader>
            <Separator />
            <SidebarContent>
                <SidebarGroup >
                    <SidebarMenu>

                        <SidebarMenuItem>
                            <SidebarMenuButton>
                                <Link href={"/"} onClick={handleSidebarToggle} className="flex w-full">Dashboard</Link>
                            </SidebarMenuButton>
                        </SidebarMenuItem>

                        <SidebarMenuItem>
                            <Separator className={"px-2 my-2"} />
                        </SidebarMenuItem>

                        <SidebarMenuItem>
                            <SidebarMenuButton>
                                <Link href={"/invoices"} onClick={handleSidebarToggle} className="flex w-full">Invoices</Link>
                            </SidebarMenuButton>
                        </SidebarMenuItem>

                        {/* <SidebarMenuItem>
                            <SidebarMenuButton>
                                <Link href={"/invoice-template"} onClick={handleSidebarToggle} className="flex w-full">Invoice Template</Link>
                            </SidebarMenuButton>
                        </SidebarMenuItem> */}

                        <SidebarMenuItem>
                            <SidebarMenuButton>
                                <Link href={"/clients"} onClick={handleSidebarToggle} className="flex w-full">Clients</Link>
                            </SidebarMenuButton>
                        </SidebarMenuItem>

                        <SidebarMenuItem>
                            <SidebarMenuButton>
                                <Link href={"/expenses"} onClick={handleSidebarToggle} className="flex w-full">Expenses</Link>
                            </SidebarMenuButton>
                        </SidebarMenuItem>

                        <SidebarMenuItem>
                            <SidebarMenuButton>
                                <Link href={"/revenues"} onClick={handleSidebarToggle} className="flex w-full">Revenues</Link>
                            </SidebarMenuButton>
                        </SidebarMenuItem>

                    </SidebarMenu>
                </SidebarGroup>
                <SidebarGroup />
            </SidebarContent>
            <SidebarFooter>
                <LogoutButton />
            </SidebarFooter>
        </Sidebar>
    )
}
