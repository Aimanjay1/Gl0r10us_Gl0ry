import { NextResponse } from "next/server";

const BACKEND_URL = process.env.BACKEND_URL || "http://localhost:5000";

export async function POST(request) {
    const { email, password } = await request.json();

    const res = await fetch(`${BACKEND_URL}/api/auth/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password }),
    });

    if (!res.ok) {
        const error = await res.json();
        return NextResponse.json({ error: error.error || "Registration failed" }, { status: res.status });
    }

    // If backend returns a token, set it as a cookie
    const data = await res.json();
    const token = data.token;
    if (token) {
        const response = NextResponse.json({ success: true });
        response.cookies.set("auth_token", token, {
            httpOnly: true,
            path: "/",
            maxAge: 60 * 60 * 24 * 7,
            sameSite: "lax",
            secure: process.env.NODE_ENV === "production",
        });
        return response;
    }

    return NextResponse.json({ error: "No token received" }, { status: 500 });
}