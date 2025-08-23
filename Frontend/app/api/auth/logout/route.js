import { NextResponse } from "next/server";

const COOKIE_NAME = process.env.COOKIE_NAME || "session";

export async function POST() {
    const res = NextResponse.json({ success: true });
    // Clear cookie (must match original attributes)
    // res.cookies.set({
    //     name: COOKIE_NAME,
    //     value: "",
    //     path: "/",
    //     httpOnly: true,
    //     sameSite: "lax",
    //     secure: process.env.NODE_ENV === "production",
    //     maxAge: 0,
    // });
    res.cookies.delete(COOKIE_NAME);
    return res;
}