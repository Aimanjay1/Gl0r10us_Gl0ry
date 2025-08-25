import { NextResponse } from "next/server";


export async function POST(request) {
    // console.log("working, returning✨✨✨")
    try {
        const { email, password } = await request.json();
        // console.log("✨✨email, password✨✨", email, password)

        const res = await fetch(`${process.env.BACKEND_URL}/api/Auth/login`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ email, password }),
        });
        // console.log("✨✨res✨✨", res)

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
        response.cookies.set(process.env.COOKIE_NAME, token, {
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