"use client";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import Link from "next/link";
import { FormEvent } from "react";
import { useRouter } from "next/navigation";

export default function Signup() {
  const router = useRouter();

  async function handleSubmit(event) {
    event.preventDefault();

    const formData = new FormData(event.currentTarget);
    const email = formData.get("email");
    const password = formData.get("password");
    const confirmPassword = formData.get("confirmPassword");

    if (password !== confirmPassword) {
      alert("Passwords do not match.");
      return;
    }

    const response = await fetch("/api/auth/register", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email, password }),
    });

    if (response.ok) {
      alert("Sign up successful.");
      router.push("/auth/login");
    } else {
      alert("Sign up failed. Please try again.");
    }
  }
  return (
    <>
      <div className="flex items-center justify-center h-screen gap-4">
        <div>
          <h1 className="text-5xl font-bold text-center">
            Tunai
            <br />
            Flow
          </h1>
          <p className="text-center">
            Your go-to financial <br /> management buddy
          </p>
        </div>
        <div>
          <form onSubmit={handleSubmit}>
            <div>
              <h1 className="text-5xl font-bold pb-3">Sign Up</h1>
              <p>Sign up to get the best experience from us.</p>
            </div>
            <div>
              <Input type="email" name="email" data-slot="input" placeholder="Email" required />
            </div>
            <div>
              <Input type="password" name="password" data-slot="input" placeholder="Password" required />
            </div>
            <div>
              <Input
                type="password"
                name="confirmPassword"
                data-slot="input"
                placeholder="Confirm Password"
                required
              />
            </div>
            <div>
              <Button data-slot="button">Sign Up</Button>
            </div>
            <div>
              ----------------------------------------------------------------
            </div>
            <div>
              <p>
                Already have an account? <Link href="/auth/login">Log in</Link>
              </p>
            </div>
          </form>
        </div>
      </div>
    </>
  );
}
