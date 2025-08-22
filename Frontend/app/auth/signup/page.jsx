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
    <div className="flex items-center justify-center min-h-screen bg-gradient-to-b from-orange-100 to-white">
      <div className="w-full max-w-md rounded-xl shadow-lg bg-white p-8">
        {/* Header */}
        <div className="text-center mb-6">
          <h1 className="text-5xl font-bold">
            Tunai<span className="text-orange-500">Flow</span>
          </h1>
          <p className="text-sm text-gray-600 mt-2">Urusan tertunai, semua flawless</p>
        </div>
        {/* Tabs */}
        <div className="flex justify-center mb-6 border-b border-orange-200">
          <Link href="/auth/login" className="px-6 py-2 text-gray-400 font-semibold border-b-2 border-transparent focus:outline-none hover:text-orange-500">
            Login
          </Link>
          <button className="px-6 py-2 text-orange-500 font-semibold border-b-2 border-orange-500 focus:outline-none cursor-default" disabled>
            Sign Up
          </button>
        </div>
        {/* Form */}
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label htmlFor="email" className="block text-sm font-medium text-gray-700">Email</label>
            <Input type="email" name="email" id="email" placeholder="Email" required className="mt-1 w-full" />
          </div>
          <div>
            <label htmlFor="password" className="block text-sm font-medium text-gray-700">Password</label>
            <Input type="password" name="password" id="password" placeholder="Password" required className="mt-1 w-full" />
          </div>
          <div>
            <label htmlFor="confirmPassword" className="block text-sm font-medium text-gray-700">Confirm Password</label>
            <Input type="password" name="confirmPassword" id="confirmPassword" placeholder="Confirm Password" required className="mt-1 w-full" />
          </div>
          <Button type="submit" className="w-full bg-orange-500 hover:bg-orange-600 text-white font-semibold py-2 rounded shadow">SIGN UP</Button>
        </form>
      </div>
    </div>
  );
}
