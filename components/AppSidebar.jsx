import {
    Sidebar,
    SidebarContent,
    SidebarFooter,
    SidebarGroup,
    SidebarHeader,
    SidebarTrigger,
} from "@/components/ui/sidebar"

export function AppSidebar() {
    return (

        <Sidebar >
            <SidebarHeader >
                {/* <div className="p-4">
                        <SidebarTrigger />
                    </div> */}
            </SidebarHeader>
            <SidebarContent>
                <SidebarGroup />
                <SidebarGroup />
            </SidebarContent>
            <SidebarFooter />
        </Sidebar>
    )
}
