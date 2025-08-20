import { NextResponse } from "next/server";
const COOKIE_NAME = "session";

export async function GET(request) {
    const loginUrl = `${request.nextUrl.origin}/login`;
    const res = NextResponse.redirect(loginUrl);
    res.cookies.set(COOKIE_NAME, "", { path: "/", maxAge: 0 });
    return res;
}