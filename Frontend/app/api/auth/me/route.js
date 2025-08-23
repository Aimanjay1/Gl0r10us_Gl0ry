import { NextResponse } from "next/server";

const BACKEND_URL = process.env.BACKEND_URL || "http://localhost:5226";

export async function GET(request) {
    try {
        const { email, password } = await request.json();
        // console.log("✨✨email, password✨✨", email, password)
        const token = request.cookies.get(process.env.COOKIE_NAME)?.value;

        const res = await fetch(`${BACKEND_URL}/api/Auth/me`, {
            method: "GET",
            headers: {
                Authorization: `Bearer ${token}`,
                "Content-Type": "application/json"
            },
        });

        if (!res.ok) {
            let error = "Login failed";
            try { error = (await res.json())?.error || error; } catch { }
            return NextResponse.json({ error }, { status: res.status });
        }

        const data = await res.json();
        // const token = data?.token;
        // if (!token) {
        //     return NextResponse.json({ error: "No token received" }, { status: 502 });
        // }

        const response = NextResponse.json(data);
        console.log("response,", response)  
        return response;
    } catch {
        return NextResponse.json({ error: "Bad request" }, { status: 400 });
    }
}