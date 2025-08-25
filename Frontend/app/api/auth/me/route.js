import { NextResponse } from "next/server";


export async function GET(request) {
    try {
        const token = request.cookies.get(process.env.COOKIE_NAME)?.value;

        const res = await fetch(`${process.env.BACKEND_URL}/api/Auth/me`, {
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
        console.log("data", data)

        return NextResponse.json(data);
    } catch (e) {
        console.log(e)
        return NextResponse.json({ error: "Bad request" }, { status: 400 });
    }
}