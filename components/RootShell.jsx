"use client"

import { usePathname } from "next/navigation"
import { SidebarInset, SidebarProvider } from "@/components/ui/sidebar"
import { AppSidebar } from "@/components/AppSidebar"
import Header from "@/components/Header"

export default function RootShell({ children }) {
    const pathname = usePathname()

    // For auth routes, don't render the app shell; let /auth layout control the UI.
    if (pathname?.startsWith("/auth")) {
        return children
    }

    // Default app shell for all other routes
    return (
        <SidebarProvider defaultOpen={false} className="flex-row">
            <AppSidebar />
            <SidebarInset>
                <Header />
                {children}
            </SidebarInset>
        </SidebarProvider>
    )
}
