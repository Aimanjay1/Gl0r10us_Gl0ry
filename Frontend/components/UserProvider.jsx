"use client";
import { useRouter } from "next/navigation";
import { createContext, useContext, useState, useEffect, useCallback, useMemo } from "react";

const UserContext = createContext(null);

export function UserProvider({ children, initialUser = null }) {
    const router = useRouter();
    const [user, setUser] = useState(initialUser);
    const [loading, setLoading] = useState(!initialUser);
    const [error, setError] = useState(null);

    // Fetch current user info
    const fetchMe = useCallback(async () => {
        setLoading(true);
        setError(null);
        try {
            const res = await fetch("/api/auth/me", { credentials: "include" });
            if (res.ok) {
                const data = await res.json();
                setUser(data);
            } else {
                setUser(null);
            }
        } catch (e) {
            setError(e);
            setUser(null);
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        if (!initialUser) fetchMe();
    }, [initialUser, fetchMe]);

    // Login helper
    const login = useCallback(
        async (email, password) => {
            setLoading(true);
            setError(null);
            try {
                const res = await fetch("/api/auth/login", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({ email, password }),
                    credentials: "include",
                });
                if (res.ok) {
                    await fetchMe();
                    return true;
                } else {
                    setError("Login failed");
                    return false;
                }
            } catch (e) {
                setError(e);
                return false;
            } finally {
                setLoading(false);
            }
        },
        [fetchMe]
    );

    // Logout helper
    const logout = useCallback(async () => {
        setLoading(true);
        setError(null);
        try {
            await fetch("/api/auth/logout", {
                method: "POST",
                credentials: "include",
            });
        } catch (e) {
            setError(e);
        } finally {
            setUser(null);
            setLoading(false);
            router.replace("/auth/login");
        }
    }, [router]);

    const value = useMemo(
        () => ({
            user,
            isAuthenticated: !!user,
            loading,
            error,
            login,
            logout,
            refresh: fetchMe,
        }),
        [user, loading, error, login, logout, fetchMe]
    );

    return <UserContext.Provider value={value}>{children}</UserContext.Provider>;
}

export function useUser() {
    const ctx = useContext(UserContext);
    if (!ctx) throw new Error("useUser must be used within UserProvider");
    return ctx;
}
