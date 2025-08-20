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
const BACKEND_URL = process.env.BACKEND_URL || "http://localhost:5226";

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

function InvoiceButton({ children }) {
    return (
        <Button className={"bg-identity-dillute hover:bg-identity"}>
            {children}
        </Button>
    )
}

export default async function Invoice(props) {

    return (
        <main className="flex flex-col h-min-full w-full p-4 lg:p-0 ">
            <div className="container mx-auto my-12">
                <h1 className="text-5xl font-bold mb-8">Add New Invoice</h1>
                <p>Generate invoices with just a click of a button</p>
            </div>

            <div className="container mx-auto">
                {/* <Link className="bg-identity object m-4 p-2 text-background rounded-sm w-fit" href={"invoices/new"}>Add New Invoice</Link> */}
            </div>

        </main>
    )
}
