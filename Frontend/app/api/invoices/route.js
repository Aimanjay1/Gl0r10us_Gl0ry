import { NextResponse } from "next/server";

const COOKIE_NAME = "session";
const BACKEND_URL = process.env.BACKEND_URL || "http://localhost:5226";

export async function GET(request) {
    const id = request.nextUrl.searchParams.get("userId");
    console.log("id", id)
    const token = request.cookies.get(COOKIE_NAME)?.value;
    if (!token) return NextResponse.json({ error: "Unauthorized" }, { status: 401 });

    const res = await fetch(`${BACKEND_URL}/api/Invoices/user/${id}`, {
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
export async function POST(request) {
    const id = request.nextUrl.searchParams.get("userId");
    const token = request.cookies.get(COOKIE_NAME)?.value;
    if (!token) return NextResponse.json({ error: "Unauthorized" }, { status: 401 });


    const body = await request.json()
    body.clientId = Number(body.clientId);
    body.userId = Number(body.userId);
    console.log("body", body)
    const res = await fetch(`${BACKEND_URL}/api/Invoices`, {
        headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
        },
        method: "POST",
        body: JSON.stringify(body),
    });
    console.log(res)

    // If Flask always returns JSON, use:
    const data = await res.json().catch(() => null);
    if (!res.ok) {
        return NextResponse.json({ error: data?.error || "Failed to create invoice" }, { status: res.status });
    }
    return NextResponse.json(data, { status: res.status });
}