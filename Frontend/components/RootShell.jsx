"use client"

import { usePathname } from "next/navigation"
import { SidebarInset, SidebarProvider } from "@/components/ui/sidebar"
import { AppSidebar } from "@/components/AppSidebar"
import Header from "@/components/Header"
import { ToastProvider } from "./Toasts"
import { RequireAuth, UserProvider } from "./UserProvider"

export default function RootShell({ children }) {
    const pathname = usePathname()

    // For auth routes, don't render the app shell; let /auth layout control the UI.
    if (pathname?.startsWith("/auth")) {
        return (
            <UserProvider>
                <ToastProvider>{children}</ToastProvider>
            </UserProvider>
        )
    }

    // Default app shell for all other routes
    return (
        <UserProvider>
            <ToastProvider>
                <SidebarProvider defaultOpen={false} className="md:flex-row overflow-x-hidden">
                    <AppSidebar />
                    <SidebarInset className="min-w-0">
                        <Header />
                        {children}
                    </SidebarInset>
                </SidebarProvider>
            </ToastProvider>
        </UserProvider>
    )
}
