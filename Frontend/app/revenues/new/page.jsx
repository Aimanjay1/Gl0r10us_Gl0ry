"use client"
import { Toaster } from "@/components/ui/sonner"
import { AppSidebar } from "@/components/AppSidebar";
import { Button } from "@/components/ui/button";
import {
    Table,
    TableBody,
    TableCaption,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
} from "@/components/ui/table"
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form";
import { useForm } from "react-hook-form";
import { Input } from "@/components/ui/input";
import { Avatar, AvatarImage, AvatarFallback } from "@/components/ui/avatar";
import { useEffect, useState } from "react";

const BACKEND_URL = process.env.BACKEND_URL || "http://localhost:5000";


function TH({ children }) {
    return (
        <TableHead className={"text-center"}>
            {children}
        </TableHead>
    )
}

function Cell({ children }) {
    return (
        <TableCell className={"text-center"}>
            {children}
        </TableCell>
    )
}

function ExpenseButton({ children }) {
    return (
        <Button className={"bg-identity-dillute hover:bg-identity"}>
            {children}
        </Button>
    )
}

export default function AddExpense(props) {
    const form = useForm({
        defaultValues: {
            profilePicture: null,
            username: "",
            companyName: "",
            companyAddress: "",
            address: "",
        },
        mode: "onSubmit",
    })

    const [preview, setPreview] = useState(null)

    useEffect(() => {
        return () => {
            if (preview) URL.revokeObjectURL(preview)
        }
    }, [preview])

    function onSubmit(values) {
        // handle submit (e.g., send to API)
        // console.log(values)
    }

    let Expenses = [];
    return (
        <main className="flex flex-col h-min-full w-full p-4 lg:p-0 ">
            <div className="container mx-auto my-12">
                <h1 className="text-5xl font-bold mb-8">Add New Expense</h1>
                <p>Generate Expenses with just a click of a button</p>
            </div>

            <div className="container mx-auto">
                {/* arrow */}
            </div>

            {/* <Badge variant={"destructive"} className={"mx-auto"}>Failed to load Expenses</Badge> */}
            <div className="container mx-auto">
                <Form {...form} >
                    <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4 max-w-md">
                        <FormField
                            control={form.control}
                            name="profilePicture"
                            render={({ field }) => (
                                <FormItem>
                                    <div className="flex gap-4">
                                        <Avatar className={"bg-accent shadow-md h-20 w-20 "}>
                                            <AvatarImage src={preview || undefined} alt="Profile preview" />
                                            <AvatarFallback>PP</AvatarFallback>
                                        </Avatar>
                                        <div className="flex flex-col justify-center gap-2">
                                            <FormLabel>
                                                Profile Picture
                                            </FormLabel>
                                            <FormControl>
                                                <Input
                                                    type={"file"}
                                                    name={field.name}
                                                    ref={field.ref}
                                                    onChange={(e) => {
                                                        const file = e.target.files?.[0] || null
                                                        field.onChange(file)
                                                        if (preview) URL.revokeObjectURL(preview)
                                                        setPreview(file ? URL.createObjectURL(file) : null)
                                                    }}
                                                    accept="image/*"
                                                />
                                            </FormControl>
                                            <FormMessage />
                                        </div>
                                    </div>
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name="username"
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Username</FormLabel>
                                    <FormControl>
                                        <Input placeholder="Expense's name" {...field} value={field.value ?? ""} />
                                    </FormControl>
                                    {/* <FormDescription>The Expense's name.</FormDescription> */}
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name="companyName"
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Company Name</FormLabel>
                                    <FormControl>
                                        <Input placeholder="Company Name" {...field} value={field.value ?? ""} />
                                    </FormControl>
                                    {/* <FormDescription>The Expense's name.</FormDescription> */}
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name="companyAddress"
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Company Address</FormLabel>
                                    <FormControl>
                                        <Input placeholder="Company Address" {...field} value={field.value ?? ""} />
                                    </FormControl>
                                    {/* <FormDescription>The Expense's name.</FormDescription> */}
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name="address"
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Company Email</FormLabel>
                                    <FormControl>
                                        <Input placeholder="Company Address" {...field} value={field.value ?? ""} />
                                    </FormControl>
                                    {/* <FormDescription>The Expense's name.</FormDescription> */}
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <Button type="submit">Save</Button>
                    </form>
                </Form>
            </div>

        </main>
    )
}

const dummyCustomers = [
    {
        ExpenseId: "INV0001",
        ExpenseId: "CS0001",
        UserId: "US0001",
        Status: "Cancelled",
        OrderDate: "DD/MM/YYYY",
        DueDate: "DD/MM/YYYY",
        TotalAmount: 1000,
        CreatedAt: "DD/MM/YYYY HH:MM:SS",

        ExpensePdfFileName: "",
        EmailMessageId: "",
        EmaiThreadId: "",
        ExpenseEmailSentAt: "",

        Expense: {},
        User: {},
        Items: {},
        Receipts: {}
    },
];