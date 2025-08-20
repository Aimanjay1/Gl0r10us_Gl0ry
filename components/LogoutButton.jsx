"use client"
import { Button } from "./ui/button";
import { useRouter } from "next/navigation";

export default function LogoutButton(props) {
    const router = useRouter();

    const handleLogout = () => {
        // Add your logout logic here
        router.push('api/auth/logout'); // Example: redirect to login page
    }

    return (
        <Button onClick={handleLogout} className={"bg-identity"}>
            Log Out
        </Button>
    )
}