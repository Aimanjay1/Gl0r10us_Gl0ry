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
    SidebarTrigger,
} from "@/components/ui/sidebar"
import Link from "next/link"

export function AppSidebar() {
    return (

        <Sidebar >
            <SidebarHeader >
                <SidebarGroup >
                    <h1 className="flex w-full font-bold text-2xl text-identity"><Link href={"/"}>GloriousGlory</Link></h1>
                </SidebarGroup>
            </SidebarHeader>
            <SidebarSeparator></SidebarSeparator>
            <SidebarContent>
                <SidebarGroup >
                    <SidebarMenu>
                        <SidebarMenuItem>
                            <SidebarMenuButton>
                                <Link href={"/"} className="flex w-full">Dashboard</Link>
                            </SidebarMenuButton>
                        </SidebarMenuItem>

                        <SidebarMenuItem>
                            <SidebarMenuButton>
                                <Link href={"/invoices"} className="flex w-full">Invoices</Link>
                            </SidebarMenuButton>
                        </SidebarMenuItem>

                        <SidebarMenuItem>
                            <SidebarMenuButton>
                                <Link href={"/invoice-template"} className="flex w-full">Invoice Template</Link>
                            </SidebarMenuButton>
                        </SidebarMenuItem>

                    </SidebarMenu>
                </SidebarGroup>
                <SidebarGroup />
            </SidebarContent>
            <SidebarFooter />
        </Sidebar>
    )
}
