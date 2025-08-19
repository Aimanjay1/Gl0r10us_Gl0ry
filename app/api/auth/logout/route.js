import { NextResponse } from "next/server";
const COOKIE_NAME = "session";

export async function GET() {
    const res = NextResponse.redirect("http://localhost:3000/auth/login");
    res.cookies.set(COOKIE_NAME, "", { path: "/", maxAge: 0 });
    return res;
}