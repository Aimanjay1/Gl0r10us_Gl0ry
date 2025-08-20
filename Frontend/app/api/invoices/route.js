import { NextResponse } from "next/server";

const COOKIE_NAME = "session";
const BACKEND_URL = process.env.BACKEND_URL || "http://localhost:5226";

export async function GET(request) {
    const body = await request.json(); // Parse JSON body
    const id = request.nextUrl.searchParams.get("userId");
    const token = request.cookies.get(COOKIE_NAME)?.value;
    if (!token) return NextResponse.json({ error: "Unauthorized" }, { status: 401 });

    const res = await fetch(`${BACKEND_URL}/api/Invoices/${id}`, {
        headers: {
            Authorization: `Bearer ${token}`,
        },
    });
    // console.log(res)

    // If Flask always returns JSON, use:
    const data = await res.json().catch(() => null);
    if (!res.ok) {
        return NextResponse.json({ error: data?.error || "Failed to fetch invoices" }, { status: res.status });
    }
    return NextResponse.json(data, { status: res.status });
}