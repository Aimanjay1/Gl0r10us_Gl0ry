"use client";
import { Button } from "./ui/button";
import { useUser } from "./UserProvider";

export default function LogoutButton(props) {
    const { logout, loading } = useUser();
    return (
        <Button onClick={logout} disabled={loading} className="bg-identity">
            {loading ? "Logging out..." : "Log Out"}
        </Button>
    );
}