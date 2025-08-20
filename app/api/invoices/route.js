import { NextResponse } from "next/server";

const COOKIE_NAME = "session";
const BACKEND_URL = process.env.BACKEND_URL || "http://localhost:5000";

export async function GET(request) {
    console.log("😂 api invoices");
    const token = request.cookies.get(COOKIE_NAME)?.value;
    console.log("session cookie", request.cookies.get(COOKIE_NAME))
    console.log("all cookie", request.cookies.getAll());
    if (!token) return NextResponse.json({ error: "Unauthorized" }, { status: 401 });

    const res = await fetch(`${BACKEND_URL}/api/invoices`, {
        headers: {
            Authorization: `Bearer ${token}`,
        },
    });
    console.log(res)

    // If Flask always returns JSON, use:
    const data = await res.json().catch(() => null);
    if (!res.ok) {
        return NextResponse.json({ error: data?.error || "Failed to fetch invoices" }, { status: res.status });
    }
    return NextResponse.json(data, { status: res.status });
}