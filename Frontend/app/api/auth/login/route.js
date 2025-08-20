import { NextResponse } from "next/server";

const COOKIE_NAME = "session";
const BACKEND_URL = process.env.BACKEND_URL || "http://localhost:5000";

export async function POST(request) {
    console.log("working, returning✨✨✨")
    try {
        const { email, password } = await request.json();

        const res = await fetch(`${BACKEND_URL}/api/auth/login`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ email, password }),
        });

        if (!res.ok) {
            let error = "Login failed";
            try { error = (await res.json())?.error || error; } catch { }
            return NextResponse.json({ error }, { status: res.status });
        }

        const data = await res.json();
        const token = data?.token;
        if (!token) {
            return NextResponse.json({ error: "No token received" }, { status: 502 });
        }

        const response = NextResponse.json({ success: true });
        response.cookies.set(COOKIE_NAME, token, {
            httpOnly: true,
            path: "/",
            maxAge: 60 * 60 * 24 * 7,
            sameSite: "lax",
            secure: process.env.NODE_ENV === "production",
            // domain: ".example.com", // only if you need cross-subdomain in prod
        });
        return response;
    } catch {
        return NextResponse.json({ error: "Bad request" }, { status: 400 });
    }
}