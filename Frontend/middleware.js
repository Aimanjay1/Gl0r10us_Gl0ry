import { NextResponse } from "next/server";
const COOKIE_NAME = "session";
const AUTH_PREFIX = "/auth";

export function middleware(request) {
    const token = request.cookies.get(COOKIE_NAME)?.value;
    const isLoggedIn = Boolean(token);
    const { pathname } = request.nextUrl;

    if (pathname.startsWith("/_next") || pathname === "/favicon.ico") return NextResponse.next();

    if (pathname.startsWith(AUTH_PREFIX)) {
        return isLoggedIn ? NextResponse.redirect(new URL("/", request.url)) : NextResponse.next();
    }

    if (!isLoggedIn) return NextResponse.redirect(new URL("/auth/login", request.url));
    return NextResponse.next();
}
export const config = {
    matcher: [
        "/((?!_next/static|_next/image|favicon.ico|api|.*\\.(?:png|jpg|svg|css|js)).*)"
    ],
};