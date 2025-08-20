"use client";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import Link from "next/link";
import { useRouter } from "next/navigation";

export default function Login() {
  const router = useRouter();

  async function handleSubmit(event) {
    event.preventDefault();
    const formData = new FormData(event.currentTarget);
    const email = formData.get("email");
    const password = formData.get("password");

    const response = await fetch("/api/auth/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email, password }),
    });

    if (response.ok) {
      router.replace("/");
    } else {
      alert("Login failed. Please check your credentials.");
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
              <h1 className="text-5xl font-bold pb-3">Log in</h1>
              <p>Welcome back!</p>
            </div>
            <div>
              <Input type="text" name="email" data-slot="input" placeholder="Username or Email" required />
            </div>
            <div>
              <Input type="password" name="password" data-slot="input" placeholder="Password" required />
            </div>
            <div>
              <Button data-slot="button">Log In</Button>
            </div>
            <div>----------------------------------------------------------------</div>
            <div><p>Don't have an account? <Link href="/auth/signup">Sign up</Link></p></div>
          </form>
        </div>
      </div>
    </>
  );
}