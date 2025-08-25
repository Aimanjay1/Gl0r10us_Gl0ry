import { NextResponse } from "next/server";

export async function POST(request) {
    const { fullName, email, password } = await request.json();

    const res = await fetch(`${process.env.BACKEND_URL}/api/Auth/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            fullName: fullName,
            companyName: "string",
            companyAddress: "string",
            email: email,
            contactNumber: "string",
            logoUrl: "string",
            password: password
        }),
    });
    console.log(res)
    if (!res.ok) {
        const error = await res.json();
        return NextResponse.json({ error: error.error || "Registration failed" }, { status: res.status });
    }

    // If backend returns a token, set it as a cookie
    const data = await res.json();
    // const token = data.token;
    // if (token) {
    //     const response = NextResponse.json({ success: true });
    //     response.cookies.set("auth_token", token, {
    //         httpOnly: true,
    //         path: "/",
    //         maxAge: 60 * 60 * 24 * 7,
    //         sameSite: "lax",
    //         secure: process.env.NODE_ENV === "production",
    //     });
    //     return response;
    // }

    return NextResponse.json({ message: "Registration successful" }, { status: res.status });
}